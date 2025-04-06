namespace SportHive.Services.Interfaces
{
 public interface IPhotoProcessing
 {
    Task<string> SavePhotoAsync(IFormFile photo);
    Task GetPhotoAsync(string path);
 }
}