using HiveServer.Models;
using CloudStructures;
using CloudStructures.Structures;

namespace HiveServer.Repository;

public interface IMemoryDB
{
	public Task<User> SetAsync(User user);
	public Task<User> GetAsync(string email);
}

public class MemoryDB : IMemoryDB
{
	readonly RedisConnection _redisConnection;
	readonly RedisString<User> _redisString;
	readonly string _key;
	readonly TimeSpan _expiryDays;
	public MemoryDB(string key, Int32 expiryDays) // TODO: 생성자에 string이나 int등을 넣지 않아야 한다.
	{
		var _redisConfig = new RedisConfig("MemoryDB", "localhost:6379");
		_redisConnection = new RedisConnection(_redisConfig);
		_key = key;
		_expiryDays = TimeSpan.FromDays(expiryDays);
		_redisString = new RedisString<User>(_redisConnection, key, _expiryDays);
	}

	public async Task<User> SetAsync(User user)
	{
		await _redisString.SetAsync(user);
		return user;
	}

	public async Task<User> GetAsync(string email)
	{
		var userResult = await _redisString.GetAsync();
		var user = userResult.Value;
		return user;
	}
}
