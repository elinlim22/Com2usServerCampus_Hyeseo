using SuperSocket;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine.Protocol;

namespace SocketServer;

public class MainServer : AppServer<ClientSession, RequestInfo>, IHostedService
{
    readonly IHostApplicationLifetime _appLifetime;
    HandlerDictionary _handlerDictionary;
    IServerConfig? _serverConfig;
    /* ----------------------------------- 생성자 ---------------------------------- */
    public MainServer(IHostApplicationLifetime appLifetime)
        : base(new DefaultReceiveFilterFactory<ReceiveFilter, RequestInfo>())
    {
        _appLifetime = appLifetime;
        _handlerDictionary = new HandlerDictionary();

        // 이 핸들러들은 AppServer를 상속받음으로써 등록해야 하는 이벤트 핸들러들이다.
        NewSessionConnected += new SessionHandler<ClientSession>(OnNewSessionConnected);
        SessionClosed += new SessionHandler<ClientSession, CloseReason>(OnSessionClosed);
        NewRequestReceived += new RequestHandler<ClientSession, RequestInfo>(OnNewRequestReceived);
    }
    /* --------------------------- Init server options -------------------------- */
    public void InitServerConfig(ServerOption options)
    {
        _serverConfig = new ServerConfig
        {
            Ip = options.Ip,
            Port = options.Port,
            MaxConnectionNumber = options.MaxConnectionNumber,
            Mode = SocketMode.Tcp,
            Name = "MainServer",
            MaxRequestLength = options.MaxRequestLength,
            ReceiveBufferSize = options.ReceiveBufferSize,
            SendBufferSize = options.SendBufferSize,
        };
    }

    public void InitHandlers() // 음...;; 처치곤란
    {
        _handlerDictionary.RegisterPacketHandler(1, (packet) =>
        {
            // var loginRequest = packet.ToPacket<LoginRequest>();
            // var loginResponse = new LoginResponse();
            // loginResponse.Result = true;
            // SendPacket(session, loginResponse);
        });

        _handlerDictionary.RegisterPacketHandler(2, (packet) =>
        {
            // var enterRoomRequest = packet.ToPacket<EnterRoomRequest>();
            // var enterRoomResponse = new EnterRoomResponse();
            // enterRoomResponse.Result = true;
            // SendPacket(session, enterRoomResponse);
        });

        _handlerDictionary.RegisterPacketHandler(3, (packet) =>
        {
            // var leaveRoomRequest = packet.ToPacket<LeaveRoomRequest>();
            // var leaveRoomResponse = new LeaveRoomResponse();
            // leaveRoomResponse.Result = true;
            // SendPacket(session, leaveRoomResponse);
        });

        _handlerDictionary.RegisterPacketHandler(4, (packet) =>
        {
            // var chatRequest = packet.ToPacket<ChatRequest>();
            // var chatResponse = new ChatResponse();
            // SendPacket(session, chatResponse);
        });
    }
    /* ------------------------------ 서버 시작 및 종료 ----------------------------- */
    public override bool Start()
    {
        InitServerConfig(new ServerOption());

        if (_serverConfig == null)
        {
            Console.WriteLine("Server configuration is not set!");
            return false;
        }

        if (!Setup(new RootConfig(), _serverConfig))
        {
            Console.WriteLine("Failed to setup the server!");
            return false;
        }

        if (!base.Start())
        {
            Console.WriteLine("Failed to start the server!");
            return false;
        }

        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopped.Register(OnStopped);

        return true;
    }
    /* -------------------------- IHostedService 메서드 구현 ------------------------- */
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() => Start(), cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() => Stop(), cancellationToken);
    }
    /* -------------------------------- 기본 핸들러 구현 ------------------------------- */
    protected override void OnStarted()
    {
        Console.WriteLine("The server has been started!");
        base.OnStarted();
    }

    protected override void OnStopped()
    {
        Console.WriteLine("The server has been stopped!");
        base.OnStopped();
    }
    /* ------------------------------- 이벤트 핸들러 구현 ------------------------------- */
    protected override void OnNewSessionConnected(ClientSession session)
    {
        Console.WriteLine($"New session connected: {session.RemoteEndPoint}");
        base.OnNewSessionConnected(session);
    }

    protected override void OnSessionClosed(ClientSession session, CloseReason reason)
    {
        Console.WriteLine($"Session closed: {session.RemoteEndPoint}");
        base.OnSessionClosed(session, reason);
    }

    protected void OnNewRequestReceived(ClientSession session, RequestInfo requestInfo)
    {
        Console.WriteLine($"New request received: {requestInfo.Key}");
        // base.OnNewRequestReceived(session, requestInfo);
    }
}
