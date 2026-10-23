using Microsoft.EntityFrameworkCore;
using waypoint_server.Configs;

namespace waypoint_server.Modules.Images
{
    public class ImagesService
    {
        public static async Task<bool> DeleteImage(int id, ApplicationContext db, IWebHostEnvironment env)
        {
            var image = await db.Set<ImagesModel>().FindAsync(id);

            if (image is null)
                throw new GraphQLException("Изображение не найдено");

            var directory = ImagesConfig.getImagePath(env);
            if (directory is null)
                throw new GraphQLException("Не удалось определить путь к изображениям");

            var filePath = System.IO.Path.Combine(directory, image.Name);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            db.Set<ImagesModel>().Remove(image);
            await db.SaveChangesAsync();

            return true;
        }
        public static async Task<List<ImageUrlModel>> GetImagesByMarkerId(int markerId, ApplicationContext db)
        {
            return await db.Set<ImagesModel>()
                .Where(x => x.MarkerId == markerId)
                .Select(x => new ImageUrlModel
                {
                    Id = x.Id,
                    Url = $"/{x.Url}/{x.Name}"
                })
                .ToListAsync();
        }
        public static async Task<bool> MoveImagesBetweenMarkers(int fromMarkerId, int toMarkerId, ApplicationContext db, IWebHostEnvironment env)
        {
            if (fromMarkerId == toMarkerId)
                throw new GraphQLException("Маркеры должны отличаться");

            try
            {
                await deleteAllMarkerImages(toMarkerId, db, env);
                await db.Set<ImagesModel>()
                    .Where(x => x.MarkerId == fromMarkerId)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(x => x.MarkerId, toMarkerId));

                return true;
            }
            catch (Exception ex)
            {
                throw new GraphQLException(
                    $"Ошибка при перемещении изображений: {ex.Message}");
            }
        }
        public static async Task<bool> deleteAllMarkerImages(int markerId, ApplicationContext db, IWebHostEnvironment env)
        {
            var directory = ImagesConfig.getImagePath(env);
            if (directory is null)
                throw new GraphQLException("Не удалось определить путь к изображениям");
            var images = await db.Set<ImagesModel>()
                .Where(x => x.MarkerId == markerId)
                .ToListAsync();
            foreach (var image in images)
            {
                var filePath = System.IO.Path.Combine(directory, image.Name);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            db.Set<ImagesModel>()
                .RemoveRange(images);
            await db.SaveChangesAsync();
            return true;
        }
        public static string? getImageByUrl(string guid, string fileName, IWebHostEnvironment env)
        {
            var directory = ImagesConfig.getImagePath(env);

            if (directory is null)
                return null;

            var filePath = System.IO.Path.Combine(directory, fileName);

            return System.IO.File.Exists(filePath) ? filePath : null;
        }
        public static async Task<bool> loadImage(IFormFile file, string directory)
        {
            var extension = System.IO.Path.GetExtension(file.FileName);
            var name = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
            var filePath = System.IO.Path.Combine(directory, $"{name}{extension}");
            await using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }
            return true;
        }
        public static async Task<bool> loadImagesWithDb(IEnumerable<IFormFile> files, int markerId, string directory, ApplicationContext db, int userId)
        {
            var images = new List<ImagesModel>();
            foreach (var file in files)
            {
                if (file.Length == 0)
                    throw new GraphQLException($"Файл {file.FileName} пустой");
                await loadImage(file, directory);
                images.Add(new ImagesModel
                {
                    Url = Guid.NewGuid().ToString(),
                    Name = file.FileName,
                    MarkerId = markerId,
                    UserId = userId
                });
            }
            db.Set<ImagesModel>().AddRange(images);
            await db.SaveChangesAsync();
            return true;
        }
    }
}
