namespace waypoint_server.Modules.Images
{
    public class ImagesConfig
    {
        public static readonly string ImagesUrlPath = "/uploads/images/";
        public static readonly string[] AllowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".webp", ".svg" };
        public static readonly long MaxFileSize = 5 * 1024 * 1024; // 5 MB
        public static string GetContentType(string path)
        {
            return System.IO.Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };
        }
        public static string? getImagePath(IWebHostEnvironment env)
        {
            return System.IO.Path.Combine(env.ContentRootPath,"uploads", "images") ?? null;
        }
    }
}
