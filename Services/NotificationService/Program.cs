using NotificationService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ConsumerEmail>();

var host = builder.Build();
host.Run();
