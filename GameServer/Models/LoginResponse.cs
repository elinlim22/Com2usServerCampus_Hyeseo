using System.ComponentModel.DataAnnotations;

namespace GameServer.Models;

public class LoginResponse(ErrorCode errorCode)
{
    [Required]
    public ErrorCode StatusCode { get; set; } = errorCode;
}
