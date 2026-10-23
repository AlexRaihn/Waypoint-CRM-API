using System.ComponentModel.DataAnnotations;
using waypoint_server.Modules.Markers;
using waypoint_server.Modules.Users;

namespace waypoint_server.Modules.Images
{
    public class ImagesModel
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int UserId { get; set; }

        [Required]
        public int MarkerId { get; set; }

        [Required]  
        public string Url { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string Name { get; set; } = string.Empty;

        [GraphQLIgnore]
        public UserModel User { get; set; } = null!;

        [GraphQLIgnore]
        public MarkerModel Marker { get; set; } = null!;
    }
    public class ImageUrlModel
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
    }
}