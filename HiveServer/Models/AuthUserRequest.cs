using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models;
public class AuthUserRequest
{
	[Required]
	[EmailAddress]
	public string Email { get; set; }
	[Required]
	public string Token { get; set; }
}
