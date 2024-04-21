using System.Security.Cryptography;
using System.Text;

namespace HiveServer.Services;

public class Security
{
	public static string HashPassword(string password, string salt)
	{
		using (var sha256 = SHA256.Create())
		{
			var saltedPassword = password + salt;
			var saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
			var hashedBytes = sha256.ComputeHash(saltedPasswordBytes);
			return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
		}
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

	public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
	{
		var hashOfEnteredPassword = HashPassword(enteredPassword, storedSalt);
		return hashOfEnteredPassword == storedHash;
	}
}
