﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SocketServer;

var builder = new HostBuilder();

builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    var env = hostingContext.HostingEnvironment;
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    // config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
    // config.AddEnvironmentVariables(prefix: "PREFIX_");
    config.AddCommandLine(args);
});

builder.ConfigureServices((hostContext, services) =>
{
    services.Configure<ServerOption>(hostContext.Configuration.GetSection("ServerOption"));
    services.Configure<ConnectionStrings>(hostContext.Configuration.GetSection("ConnectionStrings"));
    services.AddHostedService<MainServer>();
});

builder.ConfigureLogging((hostingContext, logging) =>
{
    logging.SetMinimumLevel(LogLevel.Debug);
    // logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
    logging.AddConsole();
    // logging.AddDebug();
});


var app = builder.Build();

// app.Run();
await app.RunAsync();
