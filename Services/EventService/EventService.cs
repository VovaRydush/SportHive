using DB.SportHive.Persistence;
using OperateExseption;
using SportHive.Implementations;
using SportHive.Services.Interfaces;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Persistence.Services;
using StackExchange.Redis;
using JwtAuthentication;
using Extensions;
using Microsoft.OpenApi.Models;
using DB.SportHive.MongoDb;

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
builder.Services.AddSingleton<IMongoDbService, MongoDbService>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);
builder.Services.AddScoped<IPhotoProcessing, PhotoProcessing>();

builder.Services.AddKafkaServices("localhost:9093");
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<IPhotoProcessing,PhotoProcessing>();
builder.Services.AddScoped<ISaveDataDb,SaveDataDb>();
builder.Services.AddScoped<IEventService,EventService>();
builder.Services.AddScoped<ISystemSelectionService,SystemSelectionService>();
builder.Services.AddScoped<IEventService,EventService>();

builder.Services.AddScoped<PlayOffSystem>();
builder.Services.AddScoped<DoubleEliminationSystem>();
builder.Services.AddScoped<GroupSystem>();
builder.Services.AddScoped<KnockoutSystem>();
builder.Services.AddScoped<MixsedSystem>();
builder.Services.AddScoped<OlympicSystem>();
builder.Services.AddScoped<RoundRobinSystem>();
builder.Services.AddScoped<SwissSystem>();

builder.Services.AddScoped<SystemFactory>();

builder.Services.AddScoped<DisciplineFactory>();

builder.Services.AddScoped<IEnterDataMatches, EnterDataMatches>();

builder.Services.AddScoped<SaveTeamMatch>();
builder.Services.AddScoped<SaveIndividualMatch>();
builder.Services.AddScoped<SaveExtremeMatch>();

builder.Services.AddScoped<SaveMatchFactory>();

builder.Services.AddScoped<IMatchsGenerator, MatchsGenerator>();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();       
app.UseAuthorization();

app.MapEventEndpoints();

app.Run();
