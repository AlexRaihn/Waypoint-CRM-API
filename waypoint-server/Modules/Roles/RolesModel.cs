using System.ComponentModel.DataAnnotations;
using waypoint_server.Modules.Users;

namespace waypoint_server.Modules.Roles;

public class RolesModel
{
    [Key]
    public int id { get; set; }

    [Required]
    public string name { get; set; } = null!;

    public List<UserModel> users { get; set; } = [];
}