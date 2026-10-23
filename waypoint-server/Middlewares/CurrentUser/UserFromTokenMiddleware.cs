using System.Security.Claims;
using waypoint_server.Services;

namespace waypoint_server.middlewares;

public class UserFromTokenMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        CurrentUser currentUser)
    {
        var user = context.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst("sub")?.Value;

        var role = user.FindFirst(ClaimTypes.Role)?.Value;

        if (!int.TryParse(userId, out var id) || string.IsNullOrWhiteSpace(role))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid token: missing or invalid claims");
            return;
        }

        currentUser.Id = id;
        currentUser.Role = role;
        currentUser.IsAuthenticated = true;

        await next(context);
    }
}