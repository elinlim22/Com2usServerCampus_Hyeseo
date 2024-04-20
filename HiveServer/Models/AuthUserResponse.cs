using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models;
public class AuthUserResponse(ErrorCode errorCode)
{
    [Required]
    public ErrorCode statusCode { get; set; } = errorCode;
}
