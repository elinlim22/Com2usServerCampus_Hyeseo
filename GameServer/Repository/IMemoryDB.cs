using GameServer.Models;

namespace GameServer.Repository;

public interface IMemoryDB
{
	public Task<UserGameData> SetAsync(UserGameData user, ExpiryDays expiryDays);
	public Task<UserGameData> GetAsync(string email, ExpiryDays expiryDays);
}
