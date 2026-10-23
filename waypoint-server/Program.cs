using Microsoft.EntityFrameworkCore;
using waypoint_server.App;
using waypoint_server.Configs;
using waypoint_server.middlewares;
using waypoint_server.Modules.Images;
using static waypoint_server.App.GRQLSevices;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddPooledDbContextFactory<ApplicationContext>(options =>
{
    options.UseNpgsql(connectionString);
});

ServicesAdd.init(builder);
GrphQLConfig.addGraphQLConfig(builder);

var app = builder.Build();
app.MapImagesRoutes();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseMiddleware<ImagesMiddleware>();
app.UseMiddleware<UserFromTokenMiddleware>();
app.UseAuthorization();

// Применяем миграции при запуске
using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider
        .GetRequiredService<IDbContextFactory<ApplicationContext>>();

    await using var db = await dbFactory.CreateDbContextAsync();

    await db.Database.MigrateAsync();
}

app.MapGraphQL();
app.RunWithGraphQLCommands(args);