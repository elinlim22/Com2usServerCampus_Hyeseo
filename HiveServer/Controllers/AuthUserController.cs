using HiveServer.Models;
using HiveServer.Services;
using HiveServer.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Hiveserver.Controllers;

[ApiController]
[Route("[controller]")]

public class AuthUserController : ControllerBase
{
	readonly IMemoryDB _MemoryDB;

	public AuthUserController(IMemoryDB MemoryDB)
	{
		_MemoryDB = MemoryDB;
	}

	[HttpPost]
	public async Task<AuthUserResponse> AuthUser([FromBody] AuthUserRequest _user)
	{
		var user = await _MemoryDB.GetAsync(_user.Email, 30);
		if (user == null) // 사용자 없음
		{
			return new AuthUserResponse(404);
		}
		if (user.Token != _user.Token) // 토큰 불일치
		{
			return new AuthUserResponse(401);
		}

		return new AuthUserResponse(200);
	}

}
