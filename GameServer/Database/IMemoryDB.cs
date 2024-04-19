using GameServer.Models;

namespace GameServer.Repository;

public interface IMemoryDB
{
	public Task<UserGameData> SetAsync(UserGameData user, Int32 expiryDays);
	public Task<UserGameData> GetAsync(string email, Int32 expiryDays);
}
