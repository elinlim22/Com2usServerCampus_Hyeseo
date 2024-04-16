using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HiveServer.Services;
public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string Email)
    {
        var key = Environment.GetEnvironmentVariable("JWT_SECRET");
        var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT Secret key cannot be null or empty.");
        }
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? ""));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] // 클레임이란 JWT에 담겨있는 정보를 의미한다.
        {
            new Claim(JwtRegisteredClaimNames.Sub, Email), // JwtRegisteredClaimNames.Sub : JWT의 주제를 나타내는 클레임
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // JwtRegisteredClaimNames.Jti : JWT의 고유 식별자를 나타내는 클레임
        }; // Guid란 C#에서 제공하는 고유 식별자를 나타내는 데이터 형식이다.
		//

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
            signingCredentials: credentials
        ); // JwtSecurityToken : JWT의 토큰을 나타내는 클래스
		// 토큰을 생성함

        return new JwtSecurityTokenHandler().WriteToken(token);
		// JwtSecurityTokenHandler는 JWT 토큰을 생성하고 읽는 역할을 한다.
    }
}
