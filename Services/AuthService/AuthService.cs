using DB.SportHive.Persistence;
using OperateExseption;
using SportHive.Implementations;
using SportHive.Services.Interfaces;
using AuthService.Extensions;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Persistence.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Primary")));

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

builder.Services.AddKafkaServices("localhost:9092");
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<ISaveDataDb,SaveDataDb>();
builder.Services.AddScoped<IUserRegistration, UserRegistration>();
builder.Services.AddScoped<IEmailService, EmailServiceKafka>();
builder.Services.AddScoped<IPhotoProcessing, PhotoProcessing>();
builder.Services.AddScoped<IJWTService,JWTService>();
builder.Services.AddScoped<ILoginService,LoginService>();
builder.Services.AddScoped<IProfileManipulete,ProfileManipulete>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStaticFiles();
app.MapAuthEndpoints();

app.Run();
