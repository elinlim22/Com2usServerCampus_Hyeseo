using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models;
public class CreateUserResponse(ErrorCode errorCode)
{
    [Required]
    public ErrorCode StatusCode { get; set; } = errorCode;
}
