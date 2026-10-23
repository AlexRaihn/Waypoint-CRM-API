using waypoint_server.Auth;

namespace waypoint_server.Modules.Images
{
    public class ImagesMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.Split('/');

            if (path is { Length: 4 } && path[1] == "api" && path[2] == "images" && Guid.TryParse(path[3], out _))
            {
                if (!context.Request.Cookies.ContainsKey(AuthConfig.settings.RefreshCookieName))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }

            await next(context);
        }
    }
}