using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Implementations;
using SportHive.Services.Interfaces;
using SportHive.Extensions;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Primary")));
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));

builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<IUserService, UserService>();




var app = builder.Build();

app.MapAuthEndpoints();

app.Run();