using DB.SportHive.Persistence;
using OperateExseption;
using SportHive.Implementations;
using SportHive.Services.Interfaces;
using AuthService.Extensions;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Persistence.Services;
using DB.SportHive.Domain;

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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddKafkaServices("localhost:9093");
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<ISaveDataDb,SaveDataDb>();
builder.Services.AddScoped<IUserRegistration, UserRegistration>();
builder.Services.AddScoped<IEmailService, EmailServiceKafka>();
builder.Services.AddScoped<IPhotoProcessing, PhotoProcessing>();
builder.Services.AddScoped<IJWTService,JWTService>();
builder.Services.AddScoped<ILoginService,LoginService>();
builder.Services.AddScoped<IProfileManipulete, ProfileManipulete>();
builder.Services.AddScoped<IMongoDbService, MongoDbService>();
builder.Services.AddScoped<UserProfileFactory>();
builder.Services.AddScoped<ICompliteUserProfile, CompliteUserProfile>();

builder.Services.AddScoped<FootballStats>();
builder.Services.AddScoped<AmericanFootballStats>();
builder.Services.AddScoped<ArcheryStats>();
builder.Services.AddScoped<BasketballStats>();
builder.Services.AddScoped<CheckersChessStats>();
builder.Services.AddScoped<BoxingStats>();
builder.Services.AddScoped<CyclingStats>();
builder.Services.AddScoped<IceHockeyStats>();
builder.Services.AddScoped<PowerliftingStats>();
builder.Services.AddScoped<RacketSportsStats>();
builder.Services.AddScoped<RowingStats>();
builder.Services.AddScoped<RugbyStats>();
builder.Services.AddScoped<StruggleStats>();
builder.Services.AddScoped<SwimmingStats>();
builder.Services.AddScoped<VolleyballStats>();
builder.Services.AddScoped<WeightliftingStats>();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStaticFiles();
app.MapAuthEndpoints();

app.Run();
