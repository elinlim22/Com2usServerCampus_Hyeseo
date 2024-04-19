using System.ComponentModel.DataAnnotations;

namespace GameServer.Models;

public class LoginRequest
{
	[Required]
	[EmailAddress]
	public string Email { get; set; }
	[Required]
	public string Token { get; set; }
}
