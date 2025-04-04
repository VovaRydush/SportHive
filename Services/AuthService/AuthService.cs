using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Implementations;
using SportHive.Services.Interfaces;
using SportHive.Extensions;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));

builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<IUserRegistration, UserRegistration>();
builder.Services.AddScoped<IEmailService,EmailService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapAuthEndpoints();

app.Run();