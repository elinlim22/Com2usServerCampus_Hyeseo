using System.Security.Cryptography;
using System.Text;

namespace HiveServer.Services;
public class HashData
{
	public static string Hash(string input)
	{
		var data = Encoding.ASCII.GetBytes(input); // input를 ASCII로 인코딩하여 data에 저장
		using (var sha256 = SHA256.Create()) // SHA256 객체를 생성하여 sha256에 저장
		{
			var sha256data = sha256.ComputeHash(data); // data를 해싱하여 sha256data에 저장
			return Encoding.ASCII.GetString(sha256data); // sha256data를 ASCII로 디코딩하여 반환
		} // using 문을 사용하여 sha256 객체를 사용한 후에는 자동으로 메모리에서 해제된다.
	}
}

// 솔트 생성 필요함.
