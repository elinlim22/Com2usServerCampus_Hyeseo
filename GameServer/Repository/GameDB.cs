using GameServer.Models;
using SqlKata.Execution;
using MySql.Data.MySqlClient;

namespace GameServer.Repository;

public class GameDB : IGameDB
{
	readonly IConfiguration _configuration;
	readonly QueryFactory _queryFactory;
	readonly MySqlConnection _dbConnection;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
	public GameDB(IConfiguration configuration)
	{
		_configuration = configuration;
		var connectionString = _configuration.GetConnectionString("DefaultConnection");
		var replacedConnectionString = connectionString.Replace("{myPassword}", Environment.GetEnvironmentVariable("MYSQL_PASSWORD"));
        replacedConnectionString = replacedConnectionString.Replace("{serverAddr}", Environment.GetEnvironmentVariable("SERVER_ADDR"));
        replacedConnectionString = replacedConnectionString.Replace("{mySQLPort}", Environment.GetEnvironmentVariable("MYSQL_PORT"));
		_dbConnection = new MySqlConnection(replacedConnectionString);
		_dbConnection.Open();
		_compiler = new SqlKata.Compilers.MySqlCompiler();
		_queryFactory = new QueryFactory(_dbConnection, _compiler);
	}

	public async Task<UserGameData> GetUser(string email)
	{
		var user = await _queryFactory.Query("UserGameData").Where("Email", email).FirstOrDefaultAsync<UserGameData>();
		return user;
	}
	public async Task<Int32> CreateUserGameData(UserGameData user)
	{
		var affectedRows = await _queryFactory.Query("UserGameData").InsertAsync(new
		{
			Email = user.Email,
			Level = user.Level,
			Exp = user.Exp,
			Win = user.Win,
			Lose = user.Lose
		});
		if (affectedRows == 0)
		{
			throw new Exception("Failed to create userGameData instance");
		}
		return affectedRows;
	}
	public async Task<UserGameData> UpdateUser(UserGameData user)
	{
		var affectedRows = await _queryFactory.Query("UserGameData").Where("Email", user.Email).UpdateAsync(new
		{
			Level = user.Level,
			Exp = user.Exp,
			Win = user.Win,
			Lose = user.Lose
		});
		if (affectedRows == 0)
		{
			throw new Exception("Failed to update userGameData instance");
		}
		return user;
	}
	public async Task<UserGameData> DeleteUser(string email)
	{
		var user = await GetUser(email);
		if (user == null)
		{
			throw new Exception("User not found");
		}
		var affectedRows = await _queryFactory.Query("UserGameData").Where("Email", email).DeleteAsync();
		if (affectedRows == 0)
		{
			throw new Exception("Failed to delete userGameData instance");
		}
		return user;
	}

}
