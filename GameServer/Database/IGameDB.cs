using GameServer.Models;

namespace GameServer.Repository;
public interface IGameDB
{
	Task<UserGameData> GetUser(string email);
	Task<Int32> CreateUserGameData(UserGameData user);
	Task<UserGameData?> UpdateUser(UserGameData user);
	Task<UserGameData?> DeleteUser(string email);
}
