using Microsoft.EntityFrameworkCore;
using waypoint_server.Configs;
using waypoint_server.Errors;
using waypoint_server.Modules.Users;
using HotChocolate.Authorization;

namespace waypoint_server.Auth
{
    [MutationType]
    public static partial class AuthMutation
    {
        [AllowAnonymous]
        public static async Task<AuthPayload> login(
            AuthLoginInput input,
            ApplicationContext ctx,
            [Service] AuthService auth,
            [Service] IHttpContextAccessor httpContext,
            CancellationToken ct)
        {
            await AuthValidate.ValidateUserInputForLogin(input, ctx);
            var user = await ctx.Users.AsNoTracking().Include(u => u.role).FirstOrDefaultAsync(u => u.login == input.login, ct);
            if (user == null)
                throw ErrorFactory.Unauthorized("Неверный логин или пароль");

            if (!BCrypt.Net.BCrypt.Verify(input.password, user.passwordHash))
                throw ErrorFactory.Unauthorized("Неверный логин или пароль");


            var refreshToken = await auth.CreateRefreshTokenAsync(user, ctx, ct);
            AuthService.SetRefreshCookie(httpContext.HttpContext!.Response, refreshToken);

            return new AuthPayload
            {
                accessToken = AuthService.createAccessToken(user),
                user = UsersReturnType.FromUser(user)
            };
        }
        [AllowAnonymous]
        public static async Task<bool> logout(HttpContext httpContext, ApplicationContext db, CancellationToken ct)
        {
            if (httpContext.Request.Cookies.TryGetValue(
                AuthConfig.settings.RefreshCookieName,
                out var tokenValue)
                && Guid.TryParse(tokenValue, out var refreshToken))
            {
                var user = await db.Users.FirstOrDefaultAsync(
                    u => u.refreshToken == refreshToken,
                    ct
                );

                if (user != null)
                {
                    user.refreshToken = Guid.Empty;
                    await db.SaveChangesAsync(ct);
                }
            }

            AuthService.DeleteRefreshCookie(httpContext.Response);

            return true;
        }
        [AllowAnonymous]
        public static async Task<AuthPayload> refresh(HttpContext httpContext, ApplicationContext db, CancellationToken ct)
        {
            if (!httpContext.Request.Cookies.TryGetValue(
                AuthConfig.settings.RefreshCookieName,
                out var tokenValue))
            {
                throw ErrorFactory.Unauthorized();
            }

            if (!Guid.TryParse(tokenValue, out var refreshToken))
            {
                AuthService.DeleteRefreshCookie(httpContext.Response);
                throw ErrorFactory.Unauthorized();
            }

            var user = await db.Users
            .Include(u => u.role)
            .FirstOrDefaultAsync(
                u => u.refreshToken == refreshToken,
                ct
            );

            if (user == null)
            {
                AuthService.DeleteRefreshCookie(httpContext.Response);
                throw ErrorFactory.Unauthorized();
            }

            return new AuthPayload
            {
                accessToken = AuthService.createAccessToken(user),
                user = UsersReturnType.FromUser(user)
            };
        }
        [AllowAnonymous]
        public static async Task<AuthPayload> register(
            AuthLoginInput input,
            ApplicationContext ctx,
            [Service] AuthService auth,
            [Service] IHttpContextAccessor httpContext,
            CancellationToken ct)
        {
            var newUser = new SetUsersType
            {
                login = input.login,
                password = input.password,
            };
            await UsersValidate.ValidateUserInput(newUser, ctx, true);
            var user = new UserModel
            {
                login = input.login,
                passwordHash = BCrypt.Net.BCrypt.HashPassword(input.password),
                refreshToken = Guid.Empty,
                roleId = 2
            };
            ctx.Users.Add(user);
            await ctx.Entry(user)
            .Reference(u => u.role)
            .LoadAsync(ct);

            await ctx.SaveChangesAsync(ct);
            var refreshToken = await auth.CreateRefreshTokenAsync(user, ctx, ct);
            AuthService.SetRefreshCookie(httpContext.HttpContext!.Response, refreshToken);

            return new AuthPayload
            {
                user = UsersReturnType.FromUser(user),
                accessToken = AuthService.createAccessToken(user)
            };
        }
    }
}
