using System.ComponentModel.DataAnnotations;

namespace GameServer.Models;

public class LoginResponse
{
	[Required]
	public Int64 ErrorCode { get; set; }

	public LoginResponse(Int64 errorCode)
	{
		ErrorCode = errorCode;
	}
}
