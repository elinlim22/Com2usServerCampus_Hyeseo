using Microsoft.EntityFrameworkCore;
using HiveServer.Models;

namespace HiveServer.Data.UserData;

public class UserData : DbContext
{
	public UserData(DbContextOptions<UserData> options) : base(options) // 옵션을 받아서 DbContext의 생성자 호출.
	{
	}
	// DbContext란 Entity Framework Core에서 데이터베이스와의 연결을 담당하는 클래스이다.

	public DbSet<User> Users { get; set; } // Users라는 데이터베이스 테이블 생성
}

// 여기서 Db를 연결 후 데이터베이스에 접근할 수 있게 됨. => 컨트롤러에 의존성 주입으로 데이터베이스를 연결함.
