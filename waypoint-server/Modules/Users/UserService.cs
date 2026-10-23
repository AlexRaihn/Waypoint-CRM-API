using Microsoft.EntityFrameworkCore;
using waypoint_server.Configs;

namespace waypoint_server.Modules.Users
{
    public class UserService
{
        public static async Task<UserModel?> GetUserByRefreshToken(Guid refreshToken, ApplicationContext db)
        {
            return await db.Users.FirstOrDefaultAsync(el => el.refreshToken == refreshToken);
        }
    }
}
