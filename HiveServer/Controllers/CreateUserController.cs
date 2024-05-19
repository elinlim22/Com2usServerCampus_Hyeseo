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
        _logger.ZLogInformation($"Creating user {_user.Email}");
		try
		{
            var ifExists = await _accountDB.GetUser(_user.Email);
            if (ifExists != null)
            {
                _logger.ZLogError($"User {_user.Email} already exists");
                return new CreateUserResponse(ErrorCode.UserAlreadyExists);
            }
            if (ModelState.IsValid == false)
            {
                throw new Exception("Invalid model state");
            }
            var saltValue = Security.GenerateSalt();
            var user = new User
			{
				Email = _user.Email,
				Salt = saltValue,
				Password = Security.HashPassword(_user.Password, saltValue),
				Token = _tokenService.GenerateToken(_user.Email)
			};
			await _accountDB.CreateUser(user);
			await _MemoryDB.SetAsync(user.Email, user.Token, ExpiryDays.TokenExpiry);
		}
		catch (Exception e)
		{
			_logger.ZLogError($"Error creating user {_user.Email}: {e.Message}");
			return new CreateUserResponse(ErrorCode.UserCreationFailed);
		}
		return new CreateUserResponse(ErrorCode.Success);
	}
}
