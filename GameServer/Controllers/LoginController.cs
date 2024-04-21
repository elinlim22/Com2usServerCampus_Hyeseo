using GameServer.Repository;
using GameServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ZLogger;

namespace GameServer.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
	readonly IGameDB _gameDB;
	readonly IMemoryDB _memoryDB;
	readonly IConfiguration _configuration;
	readonly ILogger<LoginController> _logger;

	public LoginController(IGameDB gameDB, IMemoryDB memoryDB, IConfiguration configuration, ILogger<LoginController> logger)
	{
		_gameDB = gameDB;
		_memoryDB = memoryDB;
		_configuration = configuration;
		_logger = logger;
	}

	[HttpPost]
	public async Task<LoginResponse> Login([FromBody] LoginRequest request)
	{
		var token = await _memoryDB.GetAsync(request.Email, ExpiryDays.RedisExpiry) ?? request.Token;
        HttpClient client = new();
		var hiveResponse = await client.PostAsJsonAsync(_configuration["HiveServer"]! + "/AuthUser",
											new { Email = request.Email, Token = token });
		if (hiveResponse == null)
		{
			_logger.ZLogError($"HiveServer is not responding : {_configuration["HiveServer"]}");
			return new LoginResponse(ErrorCode.HiveServerNotResponding);
		}
		if (hiveResponse.StatusCode != HttpStatusCode.OK)
		{
			return new LoginResponse(ErrorCode.HiveServerError);
		}
		var user = _gameDB.GetUser(request.Email);
		if (user == null)
		{
			try
			{
				var newUser = new UserGameData { Email = request.Email, Level = 1, Exp = 0, Win = 0, Lose = 0 };
				await _gameDB.CreateUserGameData(newUser);
			}
			catch (Exception e)
			{
				_logger.ZLogError($"Error creating user {request.Email}: {e.Message}");
				return new LoginResponse(ErrorCode.UserCreationFailed);
			}
		}
		await _memoryDB.SetAsync(request.Email, request.Token, ExpiryDays.RedisExpiry);
		return new LoginResponse(ErrorCode.Success);
	}
}
