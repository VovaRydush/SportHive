using JwtAuthentication;
using OperateExseption;
using DB.SportHive.Persistence;
using Extensions;
using SportHive.Services.Interfaces;
using SportHive.Implementations;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddAuthorization();
builder.Services.AddJwtAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Primary")));
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"));

builder.Services.AddScoped<IPhotoProcessing, PhotoProcessing>(); 
builder.Services.AddScoped<ISaveDataDb,SaveDataDb>();
builder.Services.AddScoped<ITeamOperateService,TeamOperateService>();
builder.Services.AddScoped<ITrainerAthletService,TrainerAthletService>();
builder.Services.AddScoped<IRedisService,RedisService>();
builder.Services.AddScoped<IGetInfoTeam, GetInfoTeam>();
builder.Services.AddScoped<IMongoDbService, MongoDbService>();
builder.Services.AddKafkaServices("localhost:9093");
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "http://localhost:5173", "http://127.0.0.1:3000", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("AllowFrontend");
app.UseAuthentication();       
app.UseAuthorization();

app.MapCommandEndpoints();

app.Run();
