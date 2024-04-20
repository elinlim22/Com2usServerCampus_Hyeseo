using HiveServer.Models;

namespace HiveServer.Repository;

public interface IMemoryDB
{
	public Task<User> SetAsync(User user, ExpiryDays expiryDays);
	public Task<User> GetAsync(string email, ExpiryDays expiryDays);
}
