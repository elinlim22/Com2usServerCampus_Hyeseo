using HiveServer.Models;
using SqlKata.Execution;
using System.Data;
using MySql.Data.MySqlClient;

namespace HiveServer.Repository;

public class AccountDB : IAccountDB
{
	readonly IConfiguration _configuration;
	readonly QueryFactory _queryFactory;
	readonly IDbConnection _dbConnection;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
	public AccountDB(IConfiguration configuration)
	{
		_configuration = configuration;
		_dbConnection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
		_dbConnection.Open();
		_compiler = new SqlKata.Compilers.MySqlCompiler();
		_queryFactory = new QueryFactory(_dbConnection, _compiler);
	}
	public async Task<Int32> CreateUser(User user)
	{
		var affectedRows = await _queryFactory.Query("Users").InsertAsync(new
		{
			Email = user.Email,
			Password = user.Password,
			Token = user.Token
		});
		return affectedRows;
	}
}
