using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameServer.Models;

public class UserGameData
{
	[Required]
	[EmailAddress]
	public string Email { get; set; }
	// 레벨, 경험치, 승, 패
	[Required]
	[Range(1, 100)]
	[DefaultValue(1)]
	public Int32 Level { get; set; }
	[Required]
	[Range(0, 100000)]
	[DefaultValue(0)]
	public Int32 Exp { get; set; }
	[Required]
	[Range(0, 100000)]
	[DefaultValue(0)]
	public Int32 Win { get; set; }
	[Required]
	[Range(0, 100000)]
	[DefaultValue(0)]
	public Int32 Lose { get; set; }
}
