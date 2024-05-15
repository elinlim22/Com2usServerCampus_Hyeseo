﻿using MemoryPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using SqlKata.Execution;
using SuperSocket.SocketBase.Config;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;
using GameServer.Models;

namespace SocketServer;

public class DBMySQLConnection
{
    bool isRunning = false;

    BufferBlock<RequestInfo> _msgBuffer = new();

    readonly IConfiguration _configuration;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
    readonly ServerOption _serverOption;
    readonly string _connectionString;

    //public Func<string, byte[], bool> SendData;
    //public Action<RequestInfo> DistributeInnerPacket;
    List<Thread> Threads = null;
    ConcurrentDictionary<int, Action<RequestInfo, QueryFactory>> PacketHandlers = [];

    public DBMySQLConnection(IConfiguration configuration, IOptions<ServerOption> serverConfig)
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
        Threads = [];
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
        // PacketHandlers[(int)PacketType.GetUserGameDataRequest] = GetUserGameData;
        PacketHandlers[(int)PacketType.SetUserGameDataRequest] = SetUserGameData;
    }

    public void InsertPacket(RequestInfo data)
    {
        _msgBuffer.Post(data);
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
                    PacketHandlers[header.Id](packet, _queryFactory);
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

    /*public void GetUserGameData(RequestInfo requestInfo, QueryFactory queryFactory)
    {
        // 이 패킷은 이너패킷이다.
        // 이 패킷(GetUserGameData)에는 유저 아이디가 들어있다. 이 아이디로 쿼리에서 찾아와야 한다.
        var reqData = MemoryPackSerializer.Deserialize<GetUserGameDataRequest>(requestInfo.Data);
        var userId = reqData.UserId;
        var userSessionId = requestInfo.SessionID;
        // 1. 쿼리로 유저게임데이터를 가져온다. 
        var userGameData = queryFactory.Query("UserGameData").Where("Email", userId).FirstOrDefaultAsync<UserGameData>();
        // 2. 해당 유저 객체에 접근해서 유저게임데이터를 저장해야 한다.
        // var user = GetUser(userSessionId); // 패킷으로 넘겨라... 패킷으로 넘겨라... 패킷으로 넘겨서 유저객체에 저장해라... 에반데
    }*/

    public void SetUserGameData(RequestInfo requestInfo, QueryFactory queryFactory)
    {
        var reqData = MemoryPackSerializer.Deserialize<SetUserGameDataRequest>(requestInfo.Data);
        var userId = reqData.UserId;
        // var userSessionId = requestInfo.SessionID;
        var affectedRows = queryFactory.Query("UserGameData").Where("Email", userId).UpdateAsync(new
        {
            Level = reqData.Level,
            Exp = reqData.Exp,
            Win = reqData.Win,
            Lose = reqData.Lose
        });
        if (affectedRows.Result == 0)
        {
            MainServer.MainLogger.Error($"MySQLProcessor - SetUserGameData: No rows affected. UserId:{userId}");
        }
    }

    public void UpdateUserGameData(RequestInfo requestInfo, QueryFactory queryFactory)
    {
        var reqData = MemoryPackSerializer.Deserialize<UpdateUserGameDataRequest>(requestInfo.Data);
        var userId = reqData.UserId;
        Task<int> affectedRows;
        if (reqData.IsWinner)
        {
            affectedRows = queryFactory.Query("UserGameData").Where("Email", userId).IncrementAsync("Win", 1);
        }
        else
        {
            affectedRows = queryFactory.Query("UserGameData").Where("Email", userId).IncrementAsync("Lose", 1);
        }
        if (affectedRows.Result == 0)
        {
            MainServer.MainLogger.Error($"MySQLProcessor - UpdateUserGameData: No rows affected. UserId:{userId}");
        }
    }
}