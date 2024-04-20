using System.ComponentModel.DataAnnotations;

namespace GameServer.Models;

public class LoginResponse(ErrorCode errorCode)
{
    [Required]
    public ErrorCode statusCode { get; set; } = errorCode;
}
