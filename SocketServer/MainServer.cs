using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
    IServerConfig _serverConfig;
    readonly PacketProcessor _packetProcessor;
    readonly RoomManager _roomManager = new();
    /* ----------------------------------- 생성자 ---------------------------------- */
    public MainServer(IHostApplicationLifetime appLifetime, ILogger<MainServer> logger)
        : base(new DefaultReceiveFilterFactory<ReceiveFilter, RequestInfo>())
    {
        _appLifetime = appLifetime;
        _appLogger = logger;
        _packetProcessor = new PacketProcessor();

        // 이 핸들러들은 AppServer를 상속받음으로써 등록해야 하는 이벤트 핸들러들이다.
        NewSessionConnected += new SessionHandler<ClientSession>(OnNewSessionConnected);
        SessionClosed += new SessionHandler<ClientSession, CloseReason>(OnSessionClosed);
        NewRequestReceived += new RequestHandler<ClientSession, RequestInfo>(OnNewRequestReceived);
    }
    /* -------------------------- IHostedService 메서드 구현 ------------------------- */
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopped.Register(OnStopped);
        return Task.Run(() => Start(), cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() => Stop(), cancellationToken);
    }
    /* -------------------------------- 기본 핸들러 구현 ------------------------------- */
    public override bool Start()
    {
        CreateComponent(new ServerOption());
        if (_serverConfig == null)
        {
            MainLogger.Error("Server configuration is not set!");
            return false;
        }

        if (!Setup(new RootConfig(), _serverConfig))
        {
            MainLogger.Error("Failed to setup the server!");
            return false;
        }

        if (!base.Start())
        {
            MainLogger.Error("Failed to start the server!");
            return false;
        }
        return true;
    }

    protected override void OnStarted()
    {
        MainLogger.Info("MainServer on start.");
        Start();
    }

    protected override void OnStopped()
    {
        MainLogger.Info("MainServer on stop.");
        Stop();
        _packetProcessor.Destory();
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
        _roomManager.CreateRooms(serverOpt);

        _packetProcessor.SendData = this.SendData;
        _packetProcessor.CreateAndStart(_roomManager.GetRoomsList(), serverOpt);

        MainLogger.Info("CreateComponent - Success");
        return ErrorCode.Success;
    }
}
