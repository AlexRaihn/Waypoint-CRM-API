using Microsoft.EntityFrameworkCore;
using waypoint_server.Configs;
using waypoint_server.Errors;

namespace waypoint_server.Modules.Users
{
    public class UsersValidate
    {
        public static async Task ValidateUserInput(SetUsersType input, ApplicationContext ctx, bool isCreate = false)
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

            if (isCreate &&
                await ctx.Users.AnyAsync(u => u.login == input.login))
            {
                throw ErrorFactory.Validation(
                    "LOGIN_ALREADY_EXISTS",
                    "Пользователь с таким логином уже существует"
                );
            }
        }
    }
}
