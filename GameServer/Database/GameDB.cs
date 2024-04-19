using GameServer.Models;
using SqlKata.Execution;
using System.Data;
using MySql.Data.MySqlClient;

namespace GameServer.Repository;

public class GameDB : IGameDB
{
	readonly IConfiguration _configuration;
	readonly QueryFactory _queryFactory;
	readonly IDbConnection _dbConnection;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
	public GameDB(IConfiguration configuration)
	{
		_configuration = configuration;
		var connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "";
		var replacedConnectionString = connectionString.Replace("{myPassword}", Environment.GetEnvironmentVariable("MYSQL_PASSWORD"));
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
		return affectedRows;
	}
	public async Task<UserGameData?> UpdateUser(UserGameData user)
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
			return null;
		}
		return user;
	}
	public async Task<UserGameData?> DeleteUser(string email)
	{
		var user = await GetUser(email);
		if (user == null)
		{
			return null;
		}
		var affectedRows = await _queryFactory.Query("UserGameData").Where("Email", email).DeleteAsync();
		if (affectedRows == 0)
		{
			return null;
		}
		return user;
	}

}
