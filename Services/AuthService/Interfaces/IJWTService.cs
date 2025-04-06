namespace SportHive.Services.Interfaces
{
 public interface IJWTService
 { 
     Task Generate(string email, string role);
 }
}