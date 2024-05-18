using HiveServer.Models;
using HiveServer.Repository;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace Hiveserver.Controllers;

[ApiController]
[Route("[controller]")]

public class AuthUserController : ControllerBase
{
	readonly IMemoryDB _MemoryDB;
	readonly ILogger<AuthUserController> _logger;
	public AuthUserController(IMemoryDB MemoryDB, ILogger<AuthUserController> logger)
	{
		_MemoryDB = MemoryDB;
		_logger = logger;
	}

	[HttpPost]
	public async Task<AuthUserResponse> AuthUser([FromBody] AuthUserRequest _user)
	{
		var userToken = await _MemoryDB.GetAsync(_user.Email, ExpiryDays.TokenExpiry);
		if (userToken == null)
		{
			_logger.ZLogError($"User {_user.Email} not found");
			return new AuthUserResponse(ErrorCode.UserNotFound);
		}
        if (userToken != _user.Token)
		{
			_logger.ZLogError($"Token mismatch for user {_user.Email}");
			return new AuthUserResponse(ErrorCode.InvalidToken);
		}
		return new AuthUserResponse(ErrorCode.Success);
	}
}
