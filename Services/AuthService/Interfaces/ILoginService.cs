namespace SportHive.Services.Interfaces
{
 public interface ILoginService
 { 
    Task Login();
    Task LogOut();
 }
}