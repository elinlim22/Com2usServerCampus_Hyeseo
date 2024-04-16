using Microsoft.EntityFrameworkCore;
using GameServer.Models;

namespace GameServer.Data.GameData;

public class GameData : DbContext
{
	public GameData(DbContextOptions<GameData> options) : base(options)
	{
	}

	public DbSet<UserGameData> UserGameDatas { get; set; }
}
