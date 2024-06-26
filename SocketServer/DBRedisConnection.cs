﻿using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Options;
using CloudStructures;
using MemoryPack;
using CloudStructures.Structures;

namespace SocketServer;

public class DBRedisConnection
{
    bool isRunning = false;

    BufferBlock<RequestInfo> _msgBuffer = new();

    readonly ServerOption _serverOption;
    readonly RedisConfig _redisConfig;

    public Func<string, byte[], bool> SendData;
    public Action<RequestInfo> DistributeInnerPacket;
    List<Thread> Threads = null;
    ConcurrentDictionary<int, Action<RequestInfo, RedisConnection>> PacketHandlers = [];

    public DBRedisConnection(ServerOption serverOption, ConnectionStrings connStr)
    {
        _serverOption = serverOption;
        var redisString = connStr.RedisConnection;
        redisString = redisString.Replace("{serverIP}", Environment.GetEnvironmentVariable("SERVER_IP"));
        redisString = redisString.Replace("{redisPort}", Environment.GetEnvironmentVariable("GAME_REDIS_PORT"));
        redisString = redisString.Replace("{redisPassword}", Environment.GetEnvironmentVariable("REDIS_PASSWORD"));

        _redisConfig = new RedisConfig("MemoryDB", redisString);

        RegistPacketHandler();
    }

    public void StartIO()
    {
        MainServer.MainLogger.Info("DBRedisConnection::StartIO - begin");
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
        PacketHandlers[(int)PacketType.ValidateUserTokenRequest] = ValidateUserToken;
    }

    public void InsertPacket(RequestInfo data)
    {
        MainServer.MainLogger.Debug($"DBRedisConnection::InsertPacket - data received: {data.Data.Length}");
        _msgBuffer.Post(data);
    }

    public void Process()
    {
        var redisConnection = new RedisConnection(_redisConfig);
        while (isRunning)
        {
            if (_msgBuffer.TryReceive(out var packet)) // 버퍼블록에서 RequestInfo 형태의 패킷을 가져온다.
            {
                MainServer.MainLogger.Debug($"DBRedisConnection::Process - packet received: {packet.Data.Length}");
                var header = new PacketHeaderInfo();
                header.Read(packet.Data);

                if (PacketHandlers.ContainsKey(header.Id))
                {
                    PacketHandlers[header.Id](packet, redisConnection);
                }
                else
                {
                    MainServer.MainLogger.Error($"RedisProcessor - packet type not found: {header.Id}");
                }
            }
            else
            {
                Thread.Sleep(1);
            }
        }
    }

    public void ValidateUserToken(RequestInfo requestInfo, RedisConnection redisConnection)
    {
        var sessionId = requestInfo.SessionID;
        var reqData = MemoryPackSerializer.Deserialize<ValidateUserTokenRequest>(requestInfo.Data);
        var token = reqData.Token;
        RedisString<string> redisString = new(redisConnection, reqData.UserId, TimeSpan.FromDays(30));
        var result = redisString.GetAsync().Result.Value;

        short errorCode;
        if (token != result)
        {
            errorCode = (short)ErrorCode.TokenNotMatched;
            MainServer.MainLogger.Error($"ValidateUserToken - token not matched: {token} != {result}");
        }
        else
        {
            errorCode = (short)ErrorCode.Success;
        }
        var innerPacket = PacketMaker.MakeValidateUserTokenResponse(reqData.UserId, errorCode, sessionId);
        DistributeInnerPacket(innerPacket);
    }
}