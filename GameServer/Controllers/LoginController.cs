using GameServer.Repository;
using GameServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ZLogger;
using Newtonsoft.Json;

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
        string hiveAddr = _configuration["HiveServer"].Replace("{hiveAddr}", Environment.GetEnvironmentVariable("HIVE_ADDR"));
		HttpClient client = new();
		var hiveResponse = await client.PostAsJsonAsync(hiveAddr + "/authuser",
											new { Email = request.Email, Token = request.Token });
		if (hiveResponse == null)
		{
			_logger.ZLogError($"HiveServer is not responding : {hiveAddr}");
			return new LoginResponse(ErrorCode.HiveServerNotResponding);
		}
        var hiveJsonResponse = hiveResponse.Content.ReadAsStringAsync().Result;
        var hiveLoginResponse = JsonConvert.DeserializeObject<AuthUserResponse>(hiveJsonResponse);
        if (hiveLoginResponse.StatusCode != (short)ErrorCode.Success)
        {
            _logger.ZLogError($"HiveServer returned status code {hiveLoginResponse.StatusCode}");
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
		var setRes = await _memoryDB.SetAsync(request.Email, request.Token, ExpiryDays.RedisExpiry);
        if (!setRes)
        {
            _logger.ZLogError($"Error setting token in Redis6400 for user {request.Email}");
            return new LoginResponse(ErrorCode.InvalidToken);
        }
        _logger.ZLogDebug($"User {request.Email}, Token {request.Token} set in Redis6400.");
		return new LoginResponse(ErrorCode.Success);
	}
}
