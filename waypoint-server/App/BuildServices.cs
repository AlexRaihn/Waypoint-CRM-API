using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using waypoint_server.Auth;
using waypoint_server.Services;

namespace waypoint_server.App
{
    public class ServicesAdd
    {
        private static void addCors(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins("http://localhost:9000")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });
        }
        public static void addAuth(WebApplicationBuilder builder)
        {
            AuthConfig.initAuthSettings(builder);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthConfig.settings.SigningKey)),
                        ValidateIssuer = true,
                        ValidIssuer = AuthConfig.settings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = AuthConfig.settings.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        RoleClaimType = System.Security.Claims.ClaimTypes.Role
                    };
                });
        }
        public static void addServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<Modules.Markers.MarkersService>();
            builder.Services.AddScoped<CurrentUser>();
        }
        public static void init(WebApplicationBuilder builder)
        {
            addServices(builder);
            addCors(builder);
            addAuth(builder);
        }
    }
}
