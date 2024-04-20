using HiveServer.Models;
using HiveServer.Services;
using HiveServer.Repository;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace HiveServer.Controllers;

[ApiController]
[Route("[controller]")]

public class CreateUserController : ControllerBase
{
	readonly TokenService _tokenService;
	readonly IAccountDB _accountDB;
	readonly IMemoryDB _MemoryDB;
	readonly ILogger<CreateUserController> _logger;

	public CreateUserController(TokenService tokenService, IAccountDB accountDB, IMemoryDB MemoryDB, ILogger<CreateUserController> logger)
	{
		_tokenService = tokenService;
		_accountDB = accountDB;
		_MemoryDB = MemoryDB;
		_logger = logger;
	}

	[HttpPost]
	public async Task<CreateUserResponse> CreateUser([FromBody] CreateUserRequest _user)
	{
		try
		{
			var user = new User
			{
				Email = _user.Email,
				Salt = Security.GenerateSalt(),
				Password = Security.HashPassword(_user.Password),
				Token = _tokenService.GenerateToken(_user.Email)
			};
			await _accountDB.CreateUser(user);
			await _MemoryDB.SetAsync(user, ExpiryDays.TokenExpiry);
		}
		catch (Exception e)
		{
			_logger.ZLogError($"Error creating user {_user.Email}: {e.Message}");
			return new CreateUserResponse(ErrorCode.UserCreationFailed);
		}

		return new CreateUserResponse(ErrorCode.Success);
	}
}
