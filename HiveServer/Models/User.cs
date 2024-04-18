

using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models;

public class User
{
	public Int32 Id { get; set; }
	[Required]
	[EmailAddress]
	public required string Email { get; set; }
	[Required]
	public string Password { get; set; }
	public string Salt { get; set; }
	public string Token { get; set; }
}


public class AuthUserRequest
{
	[Required]
	[EmailAddress]
	public string Email { get; set; }
	[Required]
	public string Token { get; set; }
}

public class AuthUserResponse
{
	[Required]
	public Int64 ErrorCode { get; set; }

	public AuthUserResponse(Int64 errorCode)
	{
		ErrorCode = errorCode;
	}
}

public class CreateUserRequest
{
	[Required]
	[EmailAddress]
	public string Email { get; set; }
	[Required]
	public string Password { get; set; }
}

public class CreateUserResponse
{
	[Required]
	public Int64 ErrorCode { get; set; }

	public CreateUserResponse(Int64 errorCode)
	{
		ErrorCode = errorCode;
	}
}
