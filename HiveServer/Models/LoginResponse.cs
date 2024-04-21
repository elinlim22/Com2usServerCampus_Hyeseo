using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models;

public class LoginResponse
{
	public string Email { get; set; }
	public string Token { get; set; }
    public ErrorCode StatusCode { get; set; }

    public LoginResponse(string email, string token, ErrorCode statusCode)
    {
        Email = email;
        Token = token;
        StatusCode = statusCode;
    }
}
