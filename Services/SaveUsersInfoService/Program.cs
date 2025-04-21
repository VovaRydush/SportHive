using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Implementations;
using SportHive.SaveServices.Interfaces;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<KafkaWorker>();
builder.Services.AddScoped<ISaveDataDb,SaveDataDb>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Primary")));

var host = builder.Build();
host.Run();
