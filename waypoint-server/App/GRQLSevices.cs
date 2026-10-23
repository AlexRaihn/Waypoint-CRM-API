using waypoint_server.Errors;
using waypoint_server.Configs;
using waypoint_server.middlewares;

namespace waypoint_server.App
{
    public class GRQLSevices
    {
        public class GrphQLConfig
        {
            public static void addCors(WebApplicationBuilder builder)
            {
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowGraphQL", policy =>
                    {
                        // это нужно для тестирования схемы передачи данных, что-то типо Swagger
                        policy.WithOrigins("http://localhost:3000")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
                });
            }
            public static void addGraphQLConfig(WebApplicationBuilder builder)
            {
                builder.Services.AddAuthorization();

                builder.AddGraphQL()
                .ModifyCostOptions(options =>
                {
                    options.Filtering.DefaultFilterArgumentCost = 1;
                    options.Filtering.DefaultFilterOperationCost = 1;
                    options.Filtering.DefaultExpensiveFilterOperationCost = 2;
                })
                .AddHttpResponseFormatter<ErrorMiddleware>()
                .AddErrorFilter(error =>
                {
                    return ErrorFactory.ErrorParser(error);
                })
                .AddTypes()
                .RegisterDbContextFactory<ApplicationContext>()
                .AddFiltering()
                .AddSorting()
                .AddProjections()
                .AddAuthorization();

                addCors(builder);
            }
    }
}
}
