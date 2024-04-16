using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameServer.Models;
using GameServer.Data.GameData;

namespace GameServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserAuthenticationController : ControllerBase
{
	private readonly GameData _gameData;

	public UserAuthenticationController(GameData gameData)
	{
		_gameData = gameData;
	}

	//
}
