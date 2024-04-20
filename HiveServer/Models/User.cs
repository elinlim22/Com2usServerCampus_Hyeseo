using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models;

public class User
{
	[Required(ErrorMessage = "Email is required")]
	[EmailAddress(ErrorMessage = "Email is not valid")]
	[StringLength(50, ErrorMessage = "Email is too long")]
	[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
	public string Email { get; set; }
	[Required]
	[MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
	[StringLength(30, ErrorMessage = "PASSWORD IS TOO LONG")]
	[DataType(DataType.Password)]
	public string Password { get; set; }
	[Required]
	public string Salt { get; set; }
	[Required]
	public string Token { get; set; }
}
