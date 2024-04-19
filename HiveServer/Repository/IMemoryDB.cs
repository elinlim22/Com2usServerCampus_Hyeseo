using HiveServer.Models;

namespace HiveServer.Repository;

public interface IMemoryDB
{
	public Task<User> SetAsync(User user, Int32 expiryDays);
	public Task<User> GetAsync(string email, Int32 expiryDays);
}
