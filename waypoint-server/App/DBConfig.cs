using Microsoft.EntityFrameworkCore;
using waypoint_server.Modules.Markers;
using waypoint_server.Modules.Roles;
using waypoint_server.Modules.Users;
using waypoint_server.Modules.Images;

namespace waypoint_server.Configs
{
    public class ApplicationContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; } = null!;
        public DbSet<MarkerModel> Markers { get; set; } = null!;
        public DbSet<RolesModel> Roles { get; set; } = null!;

        public DbSet<ImagesModel> Images { get; set; } = null!;
        
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RolesModel>().HasData(
                new RolesModel { id = 1, name = "ADMIN" },
                new RolesModel { id = 2, name = "USER" }
            );
        }
    }
}