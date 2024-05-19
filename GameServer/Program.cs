using GameServer.Repository;
using ZLogger;

var builder = WebApplication.CreateBuilder(args);

var listenAddress = builder.Configuration.GetValue<string>("ListenAddress");
listenAddress = listenAddress.Replace("{listenIP}", Environment.GetEnvironmentVariable("LISTEN_IP"));
listenAddress = listenAddress.Replace("{serverPort}", Environment.GetEnvironmentVariable("SERVER_PORT"));

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
app.Run(listenAddress);
