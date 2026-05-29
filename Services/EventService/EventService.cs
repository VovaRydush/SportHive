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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthorization();
builder.Services.AddJwtAuthentication();

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
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddKafkaServices("localhost:9093");
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<ISaveDataDb, SaveDataDb>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ISystemSelectionService, SystemSelectionService>();
builder.Services.AddScoped<INuclearRap, NuclearRap>();
builder.Services.AddScoped<DoubleEliminationSystem>();
builder.Services.AddScoped<SingleElimination>();
builder.Services.AddScoped<IGetMatchesPlayer, GetMatchesPlayer>();
builder.Services.AddScoped<GroupSystem>();
builder.Services.AddScoped<QualificationByStandard>();
builder.Services.AddScoped<RoundRobinSystem>();
builder.Services.AddScoped<SwissSystem>();
builder.Services.AddScoped<SystemFactory>();
builder.Services.AddScoped<DisciplineFactory>();
builder.Services.AddScoped<AthleteProfileFactory>();
builder.Services.AddScoped<IEnterDataMatches, EnterDataMatches>();
builder.Services.AddScoped<IEnterSportMove, EnterSportMove>();
builder.Services.AddScoped<IEnterIntermediateData, EnterIntermediateData>();
builder.Services.AddScoped<ICompliteMatch, CompliteMatch>();
builder.Services.AddScoped<ICompliteUserProfile, CompliteUserProfile>();
builder.Services.AddScoped<SaveTeamMatch>();
builder.Services.AddScoped<SaveIndividualMatch>();
builder.Services.AddScoped<SaveExtremeMatch>();
builder.Services.AddScoped<SaveMatchFactory>();
builder.Services.AddScoped<UserProfileFactory>();
builder.Services.AddScoped<EnumWork>();
builder.Services.AddScoped<ISetResultMatch, SetResultMatch>();
builder.Services.AddScoped<IMatchsGenerator, MatchsGenerator>();
builder.Services.AddScoped<IInitalSystemGrid, InitalSystemGrid>();
builder.Services.AddScoped<IGetPointMatch, GetPointMatch>();
builder.Services.AddScoped<IStage3TournamentService, Stage3TournamentService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapEventEndpoints();

app.Run();
