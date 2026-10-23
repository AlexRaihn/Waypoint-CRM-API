using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using waypoint_server.Configs;
using waypoint_server.Errors;
using waypoint_server.Modules.Images;
using waypoint_server.Services;

namespace waypoint_server.Modules.Markers;

[QueryType]
public static partial class MarkersQueries
{
    //[UsePaging]
    [UseProjection]
    [UseFiltering(typeof (MarkerFilterType))]
    [UseSorting]
    public static IQueryable<MarkerModel> GetMarkers(
        ApplicationContext ctx,
        string? search = null)
    {
        var query = ctx.Markers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";

            query = query.Where(m =>
                EF.Functions.ILike(m.Name, pattern));
        }

        return query;
    }
}

[MutationType]
public static partial class MarkersMutations
{
    public static Task<MarkerModel> CreateMarker(
        MarkerInputModel input,
        [Service] MarkersService service,
        [Service] CurrentUser user,
        CancellationToken cancellationToken)
    {
        if (user is null || user.Id is null)
        {
            throw ErrorFactory.Unauthorized();
        }
        return service.Create(user.Id.Value, input, cancellationToken);
    }

    public static async Task<MarkerModel> UpdateMarker(
        MarkerInputModel input,
        [Service] MarkersService service,
        [Service] CurrentUser user,
        CancellationToken cancellationToken)
    { 
        if(user is null || user.Id is null)
        {
            throw ErrorFactory.Unauthorized();
        }
        var marker = await service.Update(user.Id.Value, input, cancellationToken);

        return marker ?? throw ErrorFactory.NotFound();
    }

    public static async Task<bool> DeleteMarker(
        int id,
        [Service] MarkersService service,
        CancellationToken cancellationToken,
        IWebHostEnvironment env)
    {
        var deleted = await service.Delete(id, cancellationToken, env);

        if (!deleted)
        {
            throw ErrorFactory.NotFound();
        }

        return true;
    }
    public static async Task<bool> MoveMarkers(
        MarkerInputModel newMarker,
        int oldMarkerId,
        [Service] ApplicationContext db,
        [Service] MarkersService service,
        CancellationToken cancellationToken,
        IWebHostEnvironment env)
    {
        var oldMarker = await db.Markers.FindAsync(oldMarkerId);
        if(oldMarker is null)
        {
            throw ErrorFactory.NotFound();
        }
        var updatedData = new MarkerInputModel
        {
            id = oldMarker.Id,
            Name = newMarker.Name,
            Description = newMarker.Description,
            Latitude = newMarker.Latitude,
            Longitude = newMarker.Longitude
        };
        await service.Update(oldMarker.userId, updatedData, cancellationToken);
        await ImagesService.MoveImagesBetweenMarkers(newMarker.id, oldMarker.Id, db, env);
        await service.Delete(newMarker.id, cancellationToken, env);

        return true;
    }
}