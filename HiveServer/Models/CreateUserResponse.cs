using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models;
public class CreateUserResponse
{
	[Required]
	public Int64 ErrorCode { get; set; }

	public CreateUserResponse(Int64 errorCode)
	{
		ErrorCode = errorCode;
	}
}
