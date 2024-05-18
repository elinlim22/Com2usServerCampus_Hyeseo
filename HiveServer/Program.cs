using HiveServer.Services;
using HiveServer.Repository;
using ZLogger;

var builder = WebApplication.CreateBuilder(args);

var serverAddress = builder.Configuration.GetValue<string>("ServerAddress").Replace("{listenAddr}", Environment.GetEnvironmentVariable("LISTEN_ADDR"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IAccountDB, AccountDB>();
builder.Services.AddSingleton<IMemoryDB, MemoryDB>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<Security>();

builder.Logging.AddZLoggerConsole(options =>
{
    options.UseJsonFormatter();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

//app.Run();
app.Run(serverAddress);
