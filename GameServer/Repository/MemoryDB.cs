using GameServer.Models;
using CloudStructures;
using CloudStructures.Structures;
using StackExchange.Redis;

namespace GameServer.Repository;

public class MemoryDB : IMemoryDB
{
	readonly RedisConfig _redisConfig;
	readonly RedisConnection _redisConnection;
	readonly IConfiguration _configuration;
    RedisString<int> _requestSeq;
    RedisString<int> _roomToAllocate;

	public MemoryDB(IConfiguration configuration)
	{
        _configuration = configuration;
        var connStr = _configuration.GetConnectionString("RedisConnection");
        connStr = connStr.Replace("{serverIP}", Environment.GetEnvironmentVariable("SERVER_IP"));
        connStr = connStr.Replace("{redisPort}", Environment.GetEnvironmentVariable("REDIS_PORT"));
        connStr = connStr.Replace("{redisPassword}", Environment.GetEnvironmentVariable("REDIS_PASSWORD"));
        _redisConfig = new RedisConfig("MemoryDB", connStr);
        _redisConnection = new RedisConnection(_redisConfig);
        _requestSeq = new RedisString<int>(_redisConnection, "requestSeq", null);
        _roomToAllocate = new RedisString<int>(_redisConnection, "roomToAllocate", null);
    }

	public async Task<bool> SetAsync(string email, string token, ExpiryDays expiryDays)
	{
		RedisString<string> redisString = new(_redisConnection, email, TimeSpan.FromDays((double)expiryDays));
		return await redisString.SetAsync(token);
	}

	public async Task<string> GetAsync(string email, ExpiryDays expiryDays)
	{
		RedisString<string> redisString = new(_redisConnection, email, TimeSpan.FromDays((double)expiryDays));
		RedisResult<string> redisResult = await redisString.GetAsync();
		var token = redisResult.Value;
		return token;
	}

    public async Task<int> MatchRoomId()
    {
        long requestSeq = await _requestSeq.IncrementAsync(1);
        long roomNumber;
        if (requestSeq % 2 == 1)
        {
            roomNumber = await _roomToAllocate.IncrementAsync(1);
        }
        else
        {
            var i = await _roomToAllocate.GetAsync();
            roomNumber = (long)i.Value;
        }

        if (roomNumber >= 100)
        {
            await _roomToAllocate.SetAsync(0);
            roomNumber = 0;
        }
        /*
         if (requestSeq > long.MaxValue - 100)
         {
             await _requestSeq.SetAsync(0);
         }
         */

        return (int)roomNumber;
    }
}
