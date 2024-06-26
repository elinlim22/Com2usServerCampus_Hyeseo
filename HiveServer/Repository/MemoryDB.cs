using HiveServer.Models;
using CloudStructures;
using CloudStructures.Structures;

namespace HiveServer.Repository;

public class MemoryDB : IMemoryDB
{
	readonly RedisConfig _redisConfig;
	readonly RedisConnection _redisConnection;
	readonly IConfiguration _configuration;
	public MemoryDB(IConfiguration configuration)
	{
		_configuration = configuration;
        var connStr = _configuration.GetConnectionString("RedisConnection");
        connStr = connStr.Replace("{serverIP}", Environment.GetEnvironmentVariable("SERVER_IP"));
        connStr = connStr.Replace("{redisPort}", Environment.GetEnvironmentVariable("HIVE_REDIS_PORT"));
        connStr = connStr.Replace("{redisPassword}", Environment.GetEnvironmentVariable("REDIS_PASSWORD"));
		_redisConfig = new RedisConfig("MemoryDB", connStr);
		_redisConnection = new RedisConnection(_redisConfig);
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
}
