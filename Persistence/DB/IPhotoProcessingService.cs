using Microsoft.AspNetCore.Http;

namespace SportHive.Services.Interfaces
{
 public interface IPhotoProcessing
 {
    Task<string> SavePhotoAsync(IFormFile photo);
 }
}