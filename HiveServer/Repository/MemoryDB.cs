using HiveServer.Models;
using CloudStructures;
using CloudStructures.Structures;
using ZLogger;
using System.Configuration;

namespace HiveServer.Repository;

public class MemoryDB : IMemoryDB
{
	readonly RedisConfig _redisConfig;
	readonly RedisConnection _redisConnection;
	readonly ILogger<MemoryDB> _logger;
	readonly IConfiguration _configuration;
	public MemoryDB(ILogger<MemoryDB> logger, IConfiguration configuration)
	{
		_configuration = configuration;
		_redisConfig = new RedisConfig("MemoryDB", _configuration.GetConnectionString("RedisConnection") ?? "localhost:6379");
		_redisConnection = new RedisConnection(_redisConfig);
		_logger = logger;
	}

	public async Task<User> SetAsync(User user, ExpiryDays expiryDays)
	{
		RedisString<User> redisString = new(_redisConnection, user.Email, TimeSpan.FromDays(((double)expiryDays)));
		await redisString.SetAsync(user);
		return user;
	}

	public async Task<User> GetAsync(string email, ExpiryDays expiryDays)
	{
		RedisString<User> redisString = new(_redisConnection, email, TimeSpan.FromDays((double)expiryDays));
		RedisResult<User> redisResult = await redisString.GetAsync();
		var user = redisResult.Value;
		return user;
	}
}
