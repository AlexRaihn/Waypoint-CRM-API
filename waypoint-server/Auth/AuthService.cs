using Microsoft.EntityFrameworkCore;
using waypoint_server.Configs;
using waypoint_server.Modules.Users;
using System.Security.Claims;

namespace waypoint_server.Auth
{
    public class RefreshTokenType
    {
        public string token { get; set; } = string.Empty;
        public DateTimeOffset expiresAt { get; set; }
    }
    public class AuthService
    {
        public static string createAccessToken(UserModel user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(
                    ClaimTypes.Role,
                    user.role.name.ToString()
                )
            };
            var token = AuthConfig.createToken(claims);

            // Implementation for creating access token
            return token;
        }
        
        public static void SetRefreshCookie(HttpResponse response, Guid token)
        {
            response.Cookies.Append(AuthConfig.settings.RefreshCookieName, token.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = response.HttpContext.Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = AuthConfig.RefreshTokenExpiresAt,
                Path = "/"
            });
        }
        public async Task<Guid> CreateRefreshTokenAsync(UserModel user, ApplicationContext db, CancellationToken ct)
        {
            UserModel? findUser;
            try
            {
                findUser = await db.Users.FirstOrDefaultAsync(u => u.id == user.id, ct);
                if (findUser == null)
                {
                    throw new Exception("User not found");
                }
                else
                {
                    findUser.refreshToken = Guid.NewGuid();
                    await db.SaveChangesAsync(ct);
                    return findUser.refreshToken;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error finding user: " + ex.Message);
            }
        }
        public async Task<UserModel?> FindUserByRefreshTokenAsync(Guid rawToken, ApplicationContext db, CancellationToken ct)
        {
            UserModel? findUser;
            try
            {
                findUser = await db.Users.FirstOrDefaultAsync(u => u.refreshToken == rawToken, ct);
                if (findUser == null)
                {
                    throw new Exception("User not found");
                }
                return findUser;
            }
            catch (Exception ex)
            {
                throw new Exception("Error finding user: " + ex.Message);
            }
        }
        public static void DeleteRefreshCookie(HttpResponse response)
        {
            response.Cookies.Delete(
                AuthConfig.settings.RefreshCookieName,
                new CookieOptions
                {
                    Path = "/"
                }
            );
        }
    }
}
