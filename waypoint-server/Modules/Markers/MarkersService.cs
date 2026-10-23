using waypoint_server.Configs;
using waypoint_server.Modules.Images;

namespace waypoint_server.Modules.Markers;

public class MarkersService(ApplicationContext context)
{
    public async Task<MarkerModel> Create(
        int userId,
        MarkerInputModel input,
        CancellationToken cancellationToken)
    {
        var marker = new MarkerModel
        {
            Id = 0,
            Name = input.Name,
            Description = input.Description,
            Latitude = input.Latitude,
            Longitude = input.Longitude,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            userId = userId
        };

        context.Markers.Add(marker);
        await context.SaveChangesAsync(cancellationToken);

        return marker;
    }

    public async Task<MarkerModel?> Update(
        int userId,
        MarkerInputModel input,
        CancellationToken cancellationToken)
    {
        var marker = await context.Markers.FindAsync(input.id);

        if (marker is null)
            return null;

        marker.Name = input.Name;
        marker.Description = input.Description;
        marker.Latitude = input.Latitude;
        marker.Longitude = input.Longitude;
        marker.UpdatedAt = DateTime.UtcNow;
        marker.userId = userId;

        await context.SaveChangesAsync(cancellationToken);

        return marker;
    }

    public async Task<bool> Delete(
        int id,
        CancellationToken cancellationToken,
        IWebHostEnvironment env)
    {
        await ImagesService.deleteAllMarkerImages(id, context, env);
        var marker = await context.Markers.FindAsync(
            [id],
            cancellationToken
        );

        if (marker is null)
            return false;

        context.Markers.Remove(marker);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}