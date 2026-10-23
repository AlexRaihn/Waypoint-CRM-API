using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace waypoint_server.Auth
{
    public class AuthConfig
    {
        static public AuthenticationSettings settings;
        public static DateTimeOffset RefreshTokenExpiresAt => DateTimeOffset.UtcNow.AddDays(settings.RefreshTokenDays | 0);

        private static SigningCredentials createCredentionals()
        {
            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(settings.SigningKey)
                ),
                SecurityAlgorithms.HmacSha256
            );
        }
        public static string createToken(Claim[] claims)
        {
            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(settings.AccessTokenMinutes),
                signingCredentials: createCredentionals()
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static void setAuthSettings(WebApplicationBuilder builder)
        {
            settings = builder.Configuration.GetSection("Authentication").Get<AuthenticationSettings>()
                ?? throw new InvalidOperationException("Authentication configuration is missing.");

            if (Encoding.UTF8.GetByteCount(settings.SigningKey) < 32)
                throw new InvalidOperationException("Authentication:SigningKey must contain at least 32 bytes.");
        }
        public static void initAuthSettings(WebApplicationBuilder builder)
        {
            setAuthSettings(builder);
            builder.Services.AddSingleton(settings);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<AuthService>();
        }
    }
}
