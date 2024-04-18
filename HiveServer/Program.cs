using SqlKata.Execution;
using SqlKata.Compilers;
using MySql.Data.MySqlClient;
using HiveServer.Services;
using HiveServer.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// 데이터베이스 연결 문자열 설정
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
var replacedConnectionString = connectionString.Replace("{myPassword}", dbPassword);

// MySQL 컴파일러 인스턴스 생성
var compiler = new MySqlCompiler();

// 쿼리 팩토리 설정
var connection = new MySqlConnection(replacedConnectionString);
var db = new QueryFactory(connection, compiler);

// 서비스 컨테이너에 QueryFactory 인스턴스 등록
builder.Services.AddSingleton<QueryFactory>(db);
builder.Services.AddSingleton<IAccountDB, AccountDB>();
builder.Services.AddSingleton<IMemoryDB, MemoryDB>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddSingleton<HashData>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // HSTS를 사용: HTTPS를 사용하도록 강제함
}

app.UseHttpsRedirection();

app.UseStaticFiles(); // 정적 파일을 사용

app.UseRouting(); // 라우팅을 사용

app.UseAuthorization(); // 인증을 사용

app.MapControllers();

app.Run();
