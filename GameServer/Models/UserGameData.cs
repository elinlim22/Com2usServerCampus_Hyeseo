using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models;
public class UserGameData
{
	// 레벨, 경험치, 승, 패
	[ForeignKey("User")]
	public int Id { get; set; }

	public int Level { get; set; }
	public int Exp { get; set; }
	public int Win { get; set; }
	public int Lose { get; set; }
}

// 여기서도 요청객체 응답객체를 만들어야 할까?

// 클라이언트가 인증 토큰을 들고와서 로그인을 요청합니다.
// 로그인 요청(토큰) -> 하이브에 요청 보내기 -> 하이브 응답 받기 -> UserGameData 있는지 확인 -> 없으면 생성 -> 응답객체(success, UserGameData)
//                  요청객체: AuthUserRequest, 이건 패스워드도 있었는데, 필요없는듯?
//                                     응답객체: AuthUserResponse



// 솔루션 나누는건 어떻게 하지?
