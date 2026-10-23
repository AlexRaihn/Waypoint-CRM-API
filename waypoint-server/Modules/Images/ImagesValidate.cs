namespace waypoint_server.Modules.Images
{
    public class ImagesValidate
    {
        public static bool isValidImage(IFormFile file)
        {
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            return ImagesConfig.AllowedExtensions.Contains(extension) && file.Length <= ImagesConfig.MaxFileSize;
        }
        public static bool IsValidImageFiles(IEnumerable<IFormFile> files)
        {
            foreach (var file in files)
            {
                if (!isValidImage(file))
                    return false;
            }
            return true;
        }
    }
}
