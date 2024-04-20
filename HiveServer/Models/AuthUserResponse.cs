using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models;
public class AuthUserResponse(ErrorCode errorCode)
{
    [Required]
    public ErrorCode StatusCode { get; set; } = errorCode;
}
