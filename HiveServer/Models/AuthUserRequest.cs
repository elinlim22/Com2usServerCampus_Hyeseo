using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models;
public class AuthUserRequest
{
	[Required(ErrorMessage = "Email is required")]
	[EmailAddress(ErrorMessage = "Email is not valid")]
	[StringLength(50, ErrorMessage = "Email is too long")]
	[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
	public string Email { get; set; }
	[Required]
	public string Token { get; set; }
}
