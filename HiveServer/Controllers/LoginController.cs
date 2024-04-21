using HiveServer.Models;
using HiveServer.Repository;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace HiveServer.Services;

public class LoginController : ControllerBase
{
	readonly TokenService _tokenService;
	readonly IAccountDB _accountDB;
	readonly IMemoryDB _MemoryDB;
	readonly ILogger<LoginController> _logger;

	public LoginController(TokenService tokenService, IAccountDB accountDB, IMemoryDB MemoryDB, ILogger<LoginController> logger)
	{
		_tokenService = tokenService;
		_accountDB = accountDB;
		_MemoryDB = MemoryDB;
		_logger = logger;
	}

	[HttpPost]
	public async Task<LoginResponse> Login([FromBody] LoginRequest _user)
	{
		var user = await _accountDB.GetUser(_user.Email);
		if (user == null)
		{
			_logger.ZLogError($"User {_user.Email} not found");
			return new LoginResponse(_user.Email, "", ErrorCode.UserNotFound);
		}
		if (!Security.VerifyPassword(_user.Password, user.Password, user.Salt))
		{
			_logger.ZLogError($"Password mismatch for user {_user.Email}");
			return new LoginResponse(_user.Email, "", ErrorCode.InvalidPassword);
		}
		user.Token = _tokenService.GenerateToken(user.Email);
		await _MemoryDB.SetAsync(user.Email, user.Token, ExpiryDays.TokenExpiry);
		return new LoginResponse(user.Email, user.Token, ErrorCode.Success);
	}
}
