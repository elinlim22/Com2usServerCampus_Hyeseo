using HiveServer.Models;
using CloudStructures;
using CloudStructures.Structures;

namespace HiveServer.Repository;

public class MemoryDB : IMemoryDB
{
	readonly RedisConfig _redisConfig;
	readonly RedisConnection _redisConnection;
	public MemoryDB()
	{
		_redisConfig = new RedisConfig("MemoryDB", "localhost:6379");
		_redisConnection = new RedisConnection(_redisConfig);
	}

	public async Task<User> SetAsync(User user, Int32 expiryDays)
	{
		RedisString<User> redisString = new(_redisConnection, user.Email, TimeSpan.FromDays(expiryDays));
		await redisString.SetAsync(user);
		return user;
	}

	public async Task<User> GetAsync(string email, Int32 expiryDays)
	{
		RedisString<User> redisString = new(_redisConnection, email, TimeSpan.FromDays(expiryDays));
		RedisResult<User> redisResult = await redisString.GetAsync();
		var user = redisResult.Value;
		return user;
	}
}
