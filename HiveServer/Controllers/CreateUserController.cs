using HiveServer.Models;
using HiveServer.Services;
using HiveServer.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HiveServer.Controllers;

[ApiController]
[Route("[controller]")]

public class CreateUserController
{
	readonly TokenService _tokenService;
	readonly IAccountDB _accountDB;
	readonly IMemoryDB _MemoryDB;

	public CreateUserController(TokenService tokenService, IAccountDB accountDB, IMemoryDB MemoryDB)
	{
		_tokenService = tokenService;
		_accountDB = accountDB;
		_MemoryDB = MemoryDB;
	}

	[HttpPost]
	public async Task<CreateUserResponse> CreateUser(CreateUserRequest _user)
	{
		var user = new User
		{
			Email = _user.Email,
			Password = new HashData().HashPassword(_user.Password),
			Token = _tokenService.GenerateToken(_user.Email)
		};

		await _accountDB.CreateUser(user);
		await _MemoryDB.SetAsync(user);

		return new CreateUserResponse(200);
	}
}
