using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{

    public class PhotoProcessing : IPhotoProcessing
    {
        private readonly string _webRootPath;

        public PhotoProcessing(IWebHostEnvironment env, IConfiguration config)
        {
            _webRootPath = Path.Combine(env.ContentRootPath,
                                       config["WebRootPath"] ?? "wwwroot");
            Console.WriteLine($"WebRootPath: {_webRootPath}");
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
    }
}