using HiveServer.Data.UserData;
using HiveServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 토큰 서비스 추가
builder.Services.AddScoped<TokenService>();

// 환경변수에서 MySQL 비밀번호 가져오기
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
var replacedConnectionString = connectionString.Replace("{myPassword}", dbPassword);

// Entity Framework Core와 MySQL 연결을 구성
builder.Services.AddDbContext<UserData>(options =>
    options.UseMySql(replacedConnectionString,
        ServerVersion.AutoDetect(replacedConnectionString)));

// Redis 설정
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6379"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // 개발자 예외 페이지를 사용
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // HSTS를 사용: HTTPS를 사용하도록 강제함
}

app.UseHttpsRedirection(); // HTTPS 리다이렉션을 사용
app.UseStaticFiles(); // 정적 파일을 사용

app.UseRouting(); // 라우팅을 사용

app.UseAuthorization(); // 인증을 사용

// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapControllers();
// }); // 컨트롤러를 매핑해서 적용
app.MapControllers(); // 컨트롤러를 매핑해서 적용

/* UseEndpoints와 MapControllers의 차이
UseEndpoints는 라우팅을 구성하는 메서드이고, MapControllers는 컨트롤러를 라우팅에 매핑하는 메서드입니다.
UseEndpoints를 사용하면 다양한 경로와 처리기를 등록하여 라우팅을 구성할 수 있고, MapControllers를 사용하면
컨트롤러를 라우팅에 매핑하여 해당 컨트롤러의 액션 메서드가 요청을 처리할 수 있도록 할 수 있습니다.
*/

app.Run();
