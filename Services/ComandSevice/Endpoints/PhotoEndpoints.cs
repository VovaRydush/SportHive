using Microsoft.AspNetCore.StaticFiles;

namespace Command.Endpoints
{
    public static class PhotoEndpoints
    {
        public static void PhotoEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/photo", (string path, IWebHostEnvironment env) =>
            {
                return GetPhoto(path, env);
            });

            app.MapGet("/photo/{**path}", (string path, IWebHostEnvironment env) =>
            {
                return GetPhoto(path, env);
            });
        }

        private static IResult GetPhoto(string? path, IWebHostEnvironment env)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Results.NotFound();

            var fullPath = ResolvePath(path, env);

            if (fullPath is null || !File.Exists(fullPath))
                return Results.NotFound();

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(fullPath, out var contentType))
                contentType = "application/octet-stream";

            return Results.File(fullPath, contentType);
        }

        private static string? ResolvePath(string path, IWebHostEnvironment env)
        {
            var decoded = Uri.UnescapeDataString(path).Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
            var fileNameOnly = Path.GetFileName(decoded);

            var candidates = new List<string>();

            if (Path.IsPathRooted(decoded))
                candidates.Add(decoded);

            candidates.Add(Path.Combine(env.ContentRootPath, decoded));
            candidates.Add(Path.Combine(env.ContentRootPath, "wwwroot", decoded));
            candidates.Add(Path.Combine(env.ContentRootPath, "uploads", fileNameOnly));
            candidates.Add(Path.Combine(env.ContentRootPath, "wwwroot", "uploads", fileNameOnly));
            candidates.Add(Path.Combine(Directory.GetCurrentDirectory(), decoded));
            candidates.Add(Path.Combine(Directory.GetCurrentDirectory(), "uploads", fileNameOnly));
            candidates.Add(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileNameOnly));

            return candidates.FirstOrDefault(File.Exists);
        }
    }
}
