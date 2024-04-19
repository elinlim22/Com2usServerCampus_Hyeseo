using HiveServer.Models;
namespace HiveServer.Repository;

public interface IAccountDB
{
	public Task<Int32> CreateUser(User user);
}
