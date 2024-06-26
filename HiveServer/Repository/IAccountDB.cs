using HiveServer.Models;
namespace HiveServer.Repository;

public interface IAccountDB
{
	public Task<Int32> CreateUser(User user);
	public Task<User> GetUser(string email);
}
