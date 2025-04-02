using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Implementations;
using SportHive.Services.Interfaces;
using SportHive.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Primary")));

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.MapAuthEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();