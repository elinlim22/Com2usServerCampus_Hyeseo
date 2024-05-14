using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;

namespace SocketServer;

public class MainServer : AppServer<ClientSession, RequestInfo>, IHostedService
{
    public static ILog MainLogger;
    readonly ILogger<MainServer> _appLogger;
    readonly IHostApplicationLifetime _appLifetime;
    ServerOption _serverOpt;
    IServerConfig _serverConfig = null;
    readonly PacketProcessor _packetProcessor;
    readonly RoomManager _roomManager = new();
    /* ----------------------------------- 생성자 ---------------------------------- */
    public MainServer(IHostApplicationLifetime appLifetime, IOptions<ServerOption> serverConfig, ILogger<MainServer> logger)
        : base(new DefaultReceiveFilterFactory<ReceiveFilter, RequestInfo>())
    {
        _appLifetime = appLifetime;
        _appLogger = logger;
        _serverOpt = serverConfig.Value;
        _packetProcessor = new PacketProcessor(_serverOpt);

        // 이 핸들러들은 AppServer를 상속받음으로써 등록해야 하는 이벤트 핸들러들이다.
        NewSessionConnected += new SessionHandler<ClientSession>(OnNewSessionConnected);
        SessionClosed += new SessionHandler<ClientSession, CloseReason>(OnSessionClosed);
        NewRequestReceived += new RequestHandler<ClientSession, RequestInfo>(OnNewRequestReceived);
    }
    /* -------------------------- IHostedService 메서드 구현 ------------------------- */
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(AppOnStarted);
        _appLifetime.ApplicationStopped.Register(AppOnStopped);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    /* -------------------------------- 기본 핸들러 구현 ------------------------------- */
    private void AppOnStarted()
    {
        _appLogger.LogInformation("MainServer on start.");
        InitServerConfig(_serverOpt);
        CreateServer(_serverOpt);

        if (!base.Start())
        {
            _appLogger.LogError("Failed to start the server!");
            return;
        }
        else
        {
            _appLogger.LogInformation("Server started.");
        }
    }

    private void AppOnStopped()
    {
        MainLogger.Info("MainServer on stop.");
        base.Stop();
        _packetProcessor.Destory();
    }


    public void CreateServer(ServerOption serverOpt)
    {
        try
        {
            bool bResult = Setup(new RootConfig(), _serverConfig, logFactory: new NLogLogFactory());

            if (bResult == false)
            {
                MainLogger.Error("[ERROR] 서버 네트워크 설정 실패 ㅠㅠ");
                return;
            }
            else
            {
                MainLogger = base.Logger;
            }

            CreateComponent(serverOpt);
            MainLogger.Info("서버 생성 성공");
        }
        catch(Exception ex)
        {
            MainLogger.Error($"[ERROR] 서버 생성 실패: {ex.ToString()}");
        }
    }
    /* ------------------------------- 이벤트 핸들러 구현 ------------------------------- */
    protected override void OnNewSessionConnected(ClientSession session)
    {
        MainLogger.Info($"New session connected: {session.RemoteEndPoint}");
        _packetProcessor.InsertPacket(PacketMaker.MakeSessionConnectionPacket(true, session.SessionID));
    }

    protected override void OnSessionClosed(ClientSession session, CloseReason reason)
    {
        MainLogger.Info($"Session closed: {session.RemoteEndPoint}, Reason: {reason}");
        _packetProcessor.InsertPacket(PacketMaker.MakeSessionConnectionPacket(false, session.SessionID));
    }

    protected void OnNewRequestReceived(ClientSession session, RequestInfo requestInfo)
    {
        MainLogger.Info($"New request received: {requestInfo.Key}");
        requestInfo.SessionID = session.SessionID;
        _packetProcessor.InsertPacket(requestInfo);
    }
    /* ----------------------------------- 기타 ----------------------------------- */
    public bool SendData(string sessionID, byte[] sendData)
    {
        var session = GetSessionByID(sessionID);
        try
        {
            if (session == null)
            {
                return false;
            }

            session.Send(sendData, 0, sendData.Length);
        }
        catch (Exception ex)
        {
            MainLogger.Error($"{ex.ToString()},  {ex.StackTrace}");

            session.SendEndWhenSendingTimeOut();
            session.Close();
        }
        return true;
    }

    public void CloseSession(string sessionID)
    {
        var session = GetSessionByID(sessionID);
        session.Close();
    }

    public void InitServerConfig(ServerOption options)
    {
        _serverConfig = new ServerConfig
        {
            Ip = options.Ip,
            Port = options.Port,
            MaxConnectionNumber = options.MaxConnectionNumber,
            Mode = SocketMode.Tcp,
            Name = options.Name,
            MaxRequestLength = options.MaxRequestLength,
            ReceiveBufferSize = options.ReceiveBufferSize,
            SendBufferSize = options.SendBufferSize,
        };
    }
    public ErrorCode CreateComponent(ServerOption serverOpt)
    {
        InitServerConfig(serverOpt);
        Room.SendData = this.SendData;
        Room.DistributeInnerPacket = _packetProcessor.InsertPacket;
        _roomManager.CreateRooms(serverOpt);
        RoomManager.SendData = this.SendData;

        _packetProcessor.SendData = this.SendData;
        _packetProcessor.CloseSession = this.CloseSession;
        _packetProcessor.CreateAndStart(_roomManager.GetRoomsList(), serverOpt);

        MainLogger.Info("CreateComponent - Success");
        return ErrorCode.Success;
    }
}

