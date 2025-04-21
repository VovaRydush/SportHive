using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{

    public class PhotoProcessing : IPhotoProcessing
    {
        private readonly string _webRootPath;
        private IWebHostEnvironment _env;

        public PhotoProcessing(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _webRootPath = Path.Combine(env.ContentRootPath,
                                       config["WebRootPath"] ?? "wwwroot");
            Console.WriteLine($"WebRootPath: {_webRootPath}");
        }

        public async Task<string> GetPhotoBase64Async(string relativePath)
        {
            /*
            var fullPath = Path.Combine(_webRootPath, relativePath);
            if (!File.Exists(fullPath))
                return string.Empty;

            var bytes = await File.ReadAllBytesAsync(fullPath);
            var extension = Path.GetExtension(fullPath).ToLower().Trim('.');
            return $"data:image/{extension};base64,{Convert.ToBase64String(bytes)}";
            */
            return "Доробити це вже при деплоії";
        }

        public async Task<string> SavePhotoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл не був завантажений або порожній.");

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var folderPath = Path.Combine(_webRootPath, "uploads", "photos");

            try
            {
                Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, uniqueFileName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Path.Combine("uploads", "photos", uniqueFileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Помилка збереження файлу", ex);
            }
        }

        public async Task<(byte[] FileContent, string MimeType)> GetPhotoAsync(string fileName)
        {
            var path = Path.Combine(_webRootPath, "uploads", "photos", fileName);
            if (!System.IO.File.Exists(path))
                throw new FileNotFoundException("Файл не знайдено", fileName);

            var bytes = await System.IO.File.ReadAllBytesAsync(path);
            var mimeType = GetMimeType(path);

            return (bytes, mimeType);
        }

        private string GetMimeType(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }


    }
}