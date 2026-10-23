using waypoint_server.Modules.Users;

namespace waypoint_server.Auth
{
    public sealed class AuthenticationSettings
    {
        public  string RefreshCookieName = "waypoint_refresh";
        public string SigningKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenMinutes { get; set; } = 10;
        public int RefreshTokenDays { get; set; } = 30;
    }
    public record AuthLoginInput
    {
        public string login { get; set; }
        public string password { get; set; }
    }
    public record AuthPayload
    {
        public string accessToken { get; init; } = string.Empty;
        public UsersReturnType user { get; init; } = null!;
    }
}
