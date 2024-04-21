using HiveServer.Models;
using SqlKata.Execution;
using MySql.Data.MySqlClient;

namespace HiveServer.Repository;

public class AccountDB : IAccountDB
{
	readonly IConfiguration _configuration;
	readonly QueryFactory _queryFactory;
	readonly MySqlConnection _dbConnection;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
	public AccountDB(IConfiguration configuration)
	{
		_configuration = configuration;
		var connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "";
		var replacedConnectionString = connectionString.Replace("{myPassword}", Environment.GetEnvironmentVariable("MYSQL_PASSWORD"));
		_dbConnection = new MySqlConnection(replacedConnectionString);
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
		if (affectedRows == 0)
		{
			throw new Exception("Failed to create user instance");
		}
		return affectedRows;
	}
	public async Task<User> GetUser(string email)
	{
		var query = _queryFactory.Query("Users").Where("Email", email);
		var user = await query.FirstOrDefaultAsync<User>();
		return user;
	}
}
