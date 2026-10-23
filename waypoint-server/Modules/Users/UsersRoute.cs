using HotChocolate.Authorization;
using waypoint_server.Configs;
using waypoint_server.Errors;

namespace waypoint_server.Modules.Users
{
    [Authorize]
    [MutationType]
    public static partial class UsersMutation
    {
        [AllowAnonymous]
        public static async Task<UsersReturnType> setUser(
            SetUsersType input,
            ApplicationContext dbContext,
            CancellationToken ct)
        {
            await UsersValidate.ValidateUserInput(input, dbContext, true);
            var newUser = new UserModel
            {
                login = input.login,
                passwordHash = BCrypt.Net.BCrypt.HashPassword(input.password),
                createdAt = DateTime.UtcNow,
                roleId = 2,
            };

            try
            {
                dbContext.Users.Add(newUser);

                await dbContext.SaveChangesAsync(ct);
            }
            catch
            {
                throw ErrorFactory.InternalServerError();
            }

            return UsersReturnType.FromUser(newUser);
        }
    }
}