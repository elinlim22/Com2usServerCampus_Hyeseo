using HiveServer.Services;
using HiveServer.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// 데이터베이스 연결 문자열 설정
builder.Services.AddTransient<IAccountDB, AccountDB>();
builder.Services.AddSingleton<IMemoryDB, MemoryDB>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<HashData>();


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
}

app.MapControllers();

app.Run();
