using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models;

public class User
{
	[Key]
	public Int32 Id { get; set; }
	[Required]
	[EmailAddress]
	public required string Email { get; set; }
	[Required]
	public string Password { get; set; }
	[Required]
	public string Salt { get; set; }
	[Required]
	public string Token { get; set; }
}
