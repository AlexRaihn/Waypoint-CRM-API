using waypoint_server.Configs;
using waypoint_server.Enums;
using System.ComponentModel.DataAnnotations;
using waypoint_server.Modules.Roles;

namespace waypoint_server.Modules.Users
{
    public class UserModel
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string login { get; set; } = null!;

        [Required]
        [GraphQLIgnore]
        public string passwordHash { get; set; } = null!;

        public DateTime createdAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int roleId { get; set; }

        [GraphQLIgnore]
        public RolesModel role { get; set; } = null!;

        [GraphQLIgnore]
        public Guid refreshToken { get; set; } = Guid.Empty;
    }
    public class UsersReturnType
    {
        public int id { get; set; }
        public string login { get; set; }
        public string role { get; set; } = null!;
        public DateTime createdAt { get; set; }

        public static UsersReturnType FromUser(UserModel user)
        {
            return new UsersReturnType
            {
                id = user.id,
                login = user.login,
                role = user.role.name,
                createdAt = user.createdAt
            };
        }
    }
    public class SetUsersType
    {
        public string login { get; set; }
        public string password { get; set; }
    }
}
