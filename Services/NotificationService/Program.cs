using NotificationService;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<ConsumerEmail>();
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(
        builder.Configuration.GetConnectionString("Redis") ??
        builder.Configuration["Redis:ConnectionString"] ??
        "localhost:6379,abortConnect=false"));

var host = builder.Build();
host.Run();
