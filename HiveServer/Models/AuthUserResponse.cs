using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models;
public class AuthUserResponse
{
	[Required]
	public Int64 ErrorCode { get; set; }

	public AuthUserResponse(Int64 errorCode)
	{
		ErrorCode = errorCode;
	}
}
