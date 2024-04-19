using GameServer.Models;
using CloudStructures;
using CloudStructures.Structures;

namespace GameServer.Repository;

public class MemoryDB : IMemoryDB
{
	readonly RedisConfig _redisConfig;
	readonly RedisConnection _redisConnection;
	public MemoryDB()
	{
		_redisConfig = new RedisConfig("MemoryDB", "localhost:6400");
		_redisConnection = new RedisConnection(_redisConfig);
	}

	public async Task<UserGameData> SetAsync(UserGameData user, Int32 expiryDays)
	{
		RedisString<UserGameData> redisString = new(_redisConnection, user.Email, TimeSpan.FromDays(expiryDays));
		await redisString.SetAsync(user);
		return user;
	}

	public async Task<UserGameData> GetAsync(string email, Int32 expiryDays)
	{
		RedisString<UserGameData> redisString = new(_redisConnection, email, TimeSpan.FromDays(expiryDays));
		RedisResult<UserGameData> redisResult = await redisString.GetAsync();
		var user = redisResult.Value;
		return user;
	}
}
