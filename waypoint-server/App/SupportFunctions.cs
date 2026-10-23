namespace waypoint_server.App
{
    public class SupportFunctions
{
        public static Guid GetCookie(IHttpContextAccessor httpContextAccessor, string name)
        {
            return Guid.Parse(httpContextAccessor.HttpContext?
                .Request.Cookies[name] ?? Guid.Empty.ToString());
        }
    }
}
