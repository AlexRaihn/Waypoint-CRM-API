using System.ComponentModel.DataAnnotations;
using waypoint_server.Modules.Users;

namespace waypoint_server.Modules.Markers;

public class MarkerModel
{
    [Key]
    public int Id { get; set; } = 0;

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public double Latitude { get; set; } = 0;

    [Required]
    public double Longitude { get; set; } = 0;

    [Required]
    public int userId { get; set; } = 0;

    [GraphQLIgnore]
    public UserModel user { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class MarkerInputModel
{
    public int id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
}