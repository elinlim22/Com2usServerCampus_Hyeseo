using GameServer.Repository;
using ZLogger;

var builder = WebApplication.CreateBuilder(args);

var serverAddress = builder.Configuration.GetValue<string>("ServerAddress").Replace("{listenAddr}", Environment.GetEnvironmentVariable("LISTEN_ADDR"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddTransient<IGameDB, GameDB>();
builder.Services.AddSingleton<IMemoryDB, MemoryDB>();

builder.Logging.AddZLoggerConsole(options =>
{
    options.UseJsonFormatter();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

//app.Run();
app.Run(serverAddress);
