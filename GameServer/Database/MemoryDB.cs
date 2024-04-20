using GameServer.Models;
using CloudStructures;
using CloudStructures.Structures;

namespace GameServer.Repository;

public class MemoryDB : IMemoryDB
{
	readonly RedisConfig _redisConfig;
	readonly RedisConnection _redisConnection;
	readonly IConfiguration _configuration;
	public MemoryDB(IConfiguration configuration)
	{
		_configuration = configuration;
		_redisConfig = new RedisConfig("MemoryDB", _configuration.GetConnectionString("RedisConnection") ?? "localhost:6400");
		_redisConnection = new RedisConnection(_redisConfig);
	}

	public async Task<UserGameData> SetAsync(UserGameData user, ExpiryDays expiryDays)
	{
		RedisString<UserGameData> redisString = new(_redisConnection, user.Email, TimeSpan.FromDays((double)expiryDays));
		await redisString.SetAsync(user);
		return user;
	}

	public async Task<UserGameData> GetAsync(string email, ExpiryDays expiryDays)
	{
		RedisString<UserGameData> redisString = new(_redisConnection, email, TimeSpan.FromDays((double)expiryDays));
		RedisResult<UserGameData> redisResult = await redisString.GetAsync();
		var user = redisResult.Value;
		return user;
	}
}
