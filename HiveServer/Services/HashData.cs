using System.Security.Cryptography;
using System.Text;

namespace HiveServer.Services;

public class HashData
{
	public string HashPassword(string password)
	{
		using (var sha256 = SHA256.Create())
		{
			var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
			return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
		}
	}
}
