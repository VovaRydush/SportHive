using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Services.Implementations;
using DB.SportHive.Domain;
using SportHive.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Primary")));

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.MapPost("/register", async (UserInfoDto user, IUserService userService) =>
{
    await userService.Registration(user.Email, user.Password);
    return Results.Ok("User registered successfully!");
});

app.MapGet("/users", async (IUserService userService) =>
{
    List<User> users = await userService.GetAllUsers();
    return Results.Ok(users);
});

app.MapGet("/verify", async (string token, IUserService userService) => 
{
    await userService.VeryfyEmail(token);
    return Results.Ok("Верефікація пройшла успішно!");
});

app.MapGet("/", () => "Hello World!");

app.Run();