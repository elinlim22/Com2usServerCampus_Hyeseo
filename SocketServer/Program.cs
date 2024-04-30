using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SocketServer;

var builder = new HostBuilder();

builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    var env = hostingContext.HostingEnvironment;
    config.AddJsonFile("appsettings.json", optional: true);
    // config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
    // config.AddEnvironmentVariables(prefix: "PREFIX_");
    config.AddCommandLine(args);
});

builder.ConfigureServices((hostContext, services) =>
{
    services.Configure<ServerOption>(hostContext.Configuration.GetSection("ServerOption"));
    services.AddHostedService<MainServer>();
    services.AddLogging(configure => configure.AddConsole());
});

builder.ConfigureLogging((hostingContext, logging) =>
{
    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
    logging.AddConsole();
    logging.AddDebug();
});


var app = builder.Build();

app.Run();
