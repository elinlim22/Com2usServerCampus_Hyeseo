using CloudStructures.Structures;
using CloudStructures;
using HiveServer.Models;
using Microsoft.VisualBasic;
using SqlKata.Execution;

namespace HiveServer.Repository;

public interface IAccountDB
{
	public Task<Int32> CreateUser(User user);
}

public class AccountDB : IAccountDB
{
	readonly QueryFactory _queryFactory;
	public AccountDB(QueryFactory queryFactory)
	{
		_queryFactory = queryFactory;
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
