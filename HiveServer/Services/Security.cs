using System.Security.Cryptography;
using System.Text;

namespace HiveServer.Services;

public class Security
{
	public static string HashPassword(string password)
	{
		var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
		return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
	}

	public static string GenerateSalt()
	{
		var salt = new byte[32];
		using (var rng = RandomNumberGenerator.Create())
		{
			rng.GetBytes(salt);
		}
		return BitConverter.ToString(salt).Replace("-", "").ToLower();
	}
}
