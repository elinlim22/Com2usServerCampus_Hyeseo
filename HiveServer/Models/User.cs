using System;
using System.ComponentModel.DataAnnotations;


namespace HiveServer.Models;
public class User
{
	[Key]
	public Int64 Id { get; set; } // Primary Key
	[Required]
	[EmailAddress]
	public string? Email { get; set; }
	[Required]
	public string? Password { get; set; }
	// public string? Salt { get; set; } // 솔트를 여기에 저장하는게 맞을까?
	public string? Token { get; set; }

}

public class AuthUserRequest
{
	[Required]
	[EmailAddress]
	public string? Email { get; set; }
	[Required]
	public string? Password { get; set; }
	[Required]
	public string? Token { get; set; }
}

public class AuthUserResponse
{
	[Required]
	public Int64 ErrorCode { get; set; }
	public string? ErrorMessage { get; set; }

	public AuthUserResponse(Int64 errorCode, string errorMessage)
	{
		ErrorCode = errorCode;
		ErrorMessage = errorMessage;
	}
}

public class CreateUserRequest
{
	[Required]
	[EmailAddress]
	public string? Email { get; set; }
	[Required]
	public string? Password { get; set; }
}

public class CreateUserResponse
{
	[Required]
	public Int64 ErrorCode { get; set; }
	public string? ErrorMessage { get; set; }

	public CreateUserResponse(Int64 errorCode, string errorMessage)
	{
		ErrorCode = errorCode;
		ErrorMessage = errorMessage;
	}
}
