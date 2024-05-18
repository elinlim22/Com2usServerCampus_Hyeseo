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
    RedisString<int> _incr;

	public MemoryDB(IConfiguration configuration)
	{
        _configuration = configuration;
        var connStr = _configuration.GetConnectionString("RedisConnection");
        connStr = connStr.Replace("{serverAddr}", Environment.GetEnvironmentVariable("SERVER_ADDR"));
        connStr = connStr.Replace("{redisPort}", Environment.GetEnvironmentVariable("REDIS_PORT"));
        _redisConfig = new RedisConfig("MemoryDB", connStr);
        _redisConnection = new RedisConnection(_redisConfig);
        _incr = new RedisString<int>(_redisConnection, "incr", null);
    }

	public async Task<string> SetAsync(string email, string token, ExpiryDays expiryDays)
	{
		RedisString<string> redisString = new(_redisConnection, email, TimeSpan.FromDays((double)expiryDays));
		await redisString.SetAsync(token);
		return token;
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
        long roomNumber = await _incr.IncrementAsync(1);
        if (roomNumber >= 200)
        {
            await _incr.SetAsync(0);
            roomNumber = 0;
        }
        if (roomNumber % 2 == 1)
        {
            roomNumber -= 1;
        } // TODO : 방 매칭 로직 수정 필요!!

        return (int)roomNumber;
    }
}
