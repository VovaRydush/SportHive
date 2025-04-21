using Microsoft.AspNetCore.Http;

namespace SportHive.Services.Interfaces
{
 public interface IPhotoProcessing
 {
    Task<string> SavePhotoAsync(IFormFile photo);
    Task<string> GetPhotoBase64Async(string filePath);
    Task<(byte[] FileContent, string MimeType)> GetPhotoAsync(string fileName);
 }
}