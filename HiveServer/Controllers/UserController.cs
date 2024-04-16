using Microsoft.AspNetCore.Mvc;
using HiveServer.Data.UserData;
using HiveServer.Models;
using HiveServer.Services;
using StackExchange.Redis;

namespace HiveServer.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
	readonly UserData _userData;
	readonly TokenService _tokenService;
	readonly IConnectionMultiplexer _redis;

	public UserController(UserData userData, TokenService tokenService, IConnectionMultiplexer redis)
	{
		_userData = userData;
		_tokenService = tokenService;
		_redis = redis;
	} // 의존성 주입


	// 토큰 인증 api
	[HttpGet]
	public async Task<AuthUserResponse> AuthUser(AuthUserRequest request) // 토큰을 받아 redis에서 해당 유저의 토큰과 비교하고, 일치/불일치 여부 반환
	{
		var user = await _userData.Users.FindAsync(request.Token); // 토큰을 받아서 redis 데이터베이스에서 해당 토큰을 가진 유저를 찾는다.
		// redis에서 찾기
		var db = _redis.GetDatabase();
		var token = db.StringGet(request.Email);
		if (token != request.Token)
			return new AuthUserResponse(401, "Unauthorized");

		if (user == null)
			return new AuthUserResponse(404, "User not found");
		return new AuthUserResponse(200, "User found");
	}

	// 계정 생성 api
	[HttpPost]
	public async Task<CreateUserResponse> CreateUser(CreateUserRequest request)
	{

		// 1. MySQL 데이터베이스에 유저 정보 저장
		var user = new User
		{
			Email = request.Email,
			Password = request.Password // 패스워드 해싱 필요
		};
		// 2. JWT 토큰 생성
		var token = _tokenService.GenerateToken(user.Email ?? "");
		user.Token = token;
		// redis에 이메일과 토큰 저장
		var db = _redis.GetDatabase();
        db.StringSet(request.Email, token);

		await _userData.Users.AddAsync(user);
		await _userData.SaveChangesAsync();


		return new CreateUserResponse(200, "User created");
	}


}
