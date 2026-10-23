using Microsoft.EntityFrameworkCore;
using waypoint_server.Errors;
using waypoint_server.Configs;

namespace waypoint_server.Auth
{
    public class AuthValidate
    {
        public static async Task ValidateUserInputForLogin(AuthLoginInput input, ApplicationContext ctx)
        {
            if (string.IsNullOrWhiteSpace(input.login))
            {
                throw ErrorFactory.Validation(
                    "LOGIN_REQUIRED",
                    "Логин обязателен"
                );
            }
            if (string.IsNullOrWhiteSpace(input.password))
            {
                throw ErrorFactory.Validation(
                    "PASSWORD_REQUIRED",
                    "Пароль обязателен"
                );
            }
            if (await ctx.Users.FirstAsync(u => u.login == input.login) == null)
            {
                throw ErrorFactory.Validation(
                    "LOGIN_NOT_FOUND",
                    "Пользователь с таким логином не найден"
                );
            }
        }
    }
}
