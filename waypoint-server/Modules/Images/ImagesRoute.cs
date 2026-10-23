using global::waypoint_server.Configs;
using Microsoft.EntityFrameworkCore;
using waypoint_server.Modules.Markers;
using waypoint_server.Modules.Users;
using waypoint_server.Auth;
using waypoint_server.App;

namespace waypoint_server.Modules.Images
{
    public static class ImagesRoute
    {
        public static void MapImagesRoutes(this WebApplication app)
        {
            app.MapPost("/api/images", async (
                HttpRequest request,
                ApplicationContext db,
                IWebHostEnvironment env) =>
            {
                
                var refreshToken = SupportFunctions.GetCookie(app.Services.GetRequiredService<IHttpContextAccessor>(), AuthConfig.settings.RefreshCookieName);
                if(refreshToken == Guid.Empty)
                    return Results.Unauthorized();

                var user = await UserService.GetUserByRefreshToken(refreshToken, db);
                if (user is null)
                    return Results.Unauthorized();

                var form = await request.ReadFormAsync();
                var files = form.Files;

                if (files.Count == 0)
                    return Results.BadRequest("Файлы не переданы");

                if (!int.TryParse(form["markerId"], out var markerId))
                    return Results.BadRequest("Некорректный markerId");

                var markerExists = await db.Set<MarkerModel>()
                    .AnyAsync(m => m.Id == markerId);

                if (!markerExists)
                    return Results.BadRequest("Маркер не найден");

                var directory = System.IO.Path.Combine(
                    env.ContentRootPath,
                    "uploads",
                    "images"
                );
                System.IO.Directory.CreateDirectory(directory);

                await ImagesService.loadImagesWithDb(files, markerId, directory, db, user.id);

                return Results.Ok(true);
            })
            .DisableAntiforgery();

            app.MapGet("/api/images/{guid}/{fileName}", (
     string guid,
     string fileName,
     IWebHostEnvironment env) =>
            {
                var filePath = ImagesService.getImageByUrl(guid, fileName, env);

                if (filePath is null)
                    return Results.NotFound("Файл не найден");

                return Results.File(
                    filePath,
                    contentType: ImagesConfig.GetContentType(filePath)
                );
            });
        }
    }

        [MutationType]
    public static class ImagesMutation
    {
        public static async Task<bool> DeleteImage(int id, ApplicationContext db, IWebHostEnvironment env)
        {
            return await ImagesService.DeleteImage(id, db, env);
        }
        public static async Task<bool> MoveImagesBetweenMarkers(int fromMarkerId, int toMarkerId, ApplicationContext db, IWebHostEnvironment env)
        {
            return await ImagesService.MoveImagesBetweenMarkers(fromMarkerId, toMarkerId, db, env);
        }
    }
    [QueryType]
    public static class ImagesQueries
    {
        public static async Task<List<ImageUrlModel>> GetImageUrls(int markerId, ApplicationContext db)
        {
            return await ImagesService.GetImagesByMarkerId(markerId, db);
        }
    }
}
