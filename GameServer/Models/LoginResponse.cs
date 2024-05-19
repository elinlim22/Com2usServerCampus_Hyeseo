using System.ComponentModel.DataAnnotations;

namespace GameServer.Models;

public class LoginResponse(ErrorCode errorCode)
{
    public ErrorCode StatusCode { get; set; } = errorCode;
}
