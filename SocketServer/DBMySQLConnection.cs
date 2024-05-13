using MemoryPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using SqlKata.Execution;
using SuperSocket.SocketBase.Config;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace SocketServer;

public class MySQLConnection
{
    bool isRunning = false;

    BufferBlock<RequestInfo> _msgBuffer = new(); // 이 버퍼블록은 PacketProcessor에 있는 버퍼블록과 다른 것이다. mySQL연결을 위한 패킷만 여기에 담기게 된다. 마찬가지로, Redis연결을 위한 패킷은 다른 버퍼에 담기게 되겠죵?

    readonly IConfiguration _configuration;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
    readonly ServerOption _serverOption;
    readonly string _connectionString;

    public Func<string, byte[], bool> SendData;
    public Action<RequestInfo> DistributeInnerPacket;
    List<Thread> Threads = null;
    ConcurrentDictionary<int, Func<RequestInfo, QueryFactory, /*Packet*/>> PacketHandlers = [];

    public MySQLConnection(IConfiguration configuration, IOptions<ServerOption> serverConfig)
    {
        _configuration = configuration;
        _serverOption = serverConfig.Value;

        var toReplace = _configuration.GetConnectionString("DefaultConnection") ?? "";
        _connectionString = toReplace.Replace("{myPassword}", Environment.GetEnvironmentVariable("MYSQL_PASSWORD"));
        
        _compiler = new SqlKata.Compilers.MySqlCompiler();

        RegistPacketHandler();
    }

    public void StartIO()
    {
        Threads = new List<Thread>();
        isRunning = true;
        for (int i = 0; i < _serverOption.MaxThread; i++)
        {
            var thread = new Thread(Process);
            Threads.Add(thread);
            thread.Start();
        }
    }

    void RegistPacketHandler()
    {
        PacketHandlers[(int)PacketType.GetUserGameData] = GetUserGameData; // >> 예시입니다~ 패킷타입과 핸들러를 만들어야 한다!
    }

    public void Process()
    {
        var _dbConnection = new MySqlConnection(_connectionString);
        _dbConnection.Open();
        var _queryFactory = new QueryFactory(_dbConnection, _compiler);
        while (isRunning)
        {
            if (_msgBuffer.TryReceive(out var packet)) // 버퍼블록에서 RequestInfo 형태의 패킷을 가져온다.
            {
                var header = new PacketHeaderInfo();
                header.Read(packet.Data);

                if (PacketHandlers.ContainsKey(header.Id))
                {
                    var data = PacketHandlers[header.Id](packet, _queryFactory);
                }
                else
                {
                    MainServer.MainLogger.Error($"MySQLProcessor - packet type not found: {header.Id}");
                }
            }
            else
            {
                Thread.Sleep(1);
            }
        }
    }

    // 패킷 핸들러 목록

    /*Packet GetUserGameData(RequestInfo requestInfo, QueryFactory queryFactory)
    {
        var reqData = MemoryPackSerializer.Deserialize<GetUserGameData>(requestInfo.Data);
        var userId = reqData.UserId;
        var user = queryFactory.Query("UserGameData").Where("Email", userId).FirstOrDefaultAsync<UserGameData>();
        return user;
    }*/
}