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
		_redisConfig = new RedisConfig("MemoryDB", _configuration.GetConnectionString("RedisConnection") ?? "localhost:6379");
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
