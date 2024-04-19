using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models;
public class CreateUserRequest
{
	[Required]
	[EmailAddress]
	public string Email { get; set; }
	[Required]
	public string Password { get; set; }
}
