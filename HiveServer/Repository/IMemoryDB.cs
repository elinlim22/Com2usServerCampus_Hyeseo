using HiveServer.Models;

namespace HiveServer.Repository;

public interface IMemoryDB
{
	public Task<string> SetAsync(string email, string token, ExpiryDays expiryDays);
	public Task<string> GetAsync(string email, ExpiryDays expiryDays);
}
