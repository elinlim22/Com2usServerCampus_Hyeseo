using GameServer.Repository;
using GameServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GameServer.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
	readonly IGameDB _gameDB;
	readonly IMemoryDB _memoryDB;
	readonly IConfiguration _configuration;

	public LoginController(IGameDB gameDB, IMemoryDB memoryDB, IConfiguration configuration)
	{
		_gameDB = gameDB;
		_memoryDB = memoryDB;
		_configuration = configuration;
	}

	[HttpPost]
	public async Task<LoginResponse> Login([FromBody] LoginRequest request)
	{
		HttpClient client = new();
		var hiveResponse = await client.PostAsJsonAsync(_configuration["HiveServer"]! + "/AuthUser",
											new { Email = request.Email, Token = request.Token });
		if (hiveResponse == null)
		{
			return new LoginResponse(4); // TODO : ErrorCode 정의하기
		}
		if (hiveResponse.StatusCode != HttpStatusCode.OK)
		{
			Console.WriteLine($"HiveServer Error : {hiveResponse.StatusCode}");
			return new LoginResponse(3); // TODO : ErrorCode 정의하기
		}
		var user = _gameDB.GetUser(request.Email);
		if (user == null)
		{
			var newUser = new UserGameData { Email = request.Email, Level = 1, Exp = 0, Win = 0, Lose = 0 };
			await _gameDB.CreateUserGameData(newUser);
			await _memoryDB.SetAsync(newUser, 30); // TODO : ExpiryDays 정의하기
			return new LoginResponse(0);
		}
		return new LoginResponse(0);
	}
}
