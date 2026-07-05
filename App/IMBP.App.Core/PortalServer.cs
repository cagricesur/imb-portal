using IMBP.App.Core.Services;
using IMBP.App.Domain.Settings;
using IMBP.App.Domain.Specifications;
using IMBP.App.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;

namespace IMBP.App.Core
{
    public class PortalServer
    {
        public static void Run(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            builder.Services.AddOpenApi();

            builder.ConfigurePortalSettings(out var jwtSettings);
            builder.ConfigurePortalContext();
            builder.ConfigurePortalServices();

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs/portal"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2),
                };
            });

            var app = builder.Build();

            app.UseForwardedHeaders();
            app.UseDefaultFiles();
            app.MapStaticAssets();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<PortalHub>("/hubs/portal");
            app.MapFallbackToFile("/index.html");

            app.Run();

        }
    }

    internal static class WebServerExtensions
    {
        internal static void ConfigurePortalContext(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<PortalContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("PortalContext"));
            });
        }
        internal static void ConfigurePortalSettings(this WebApplicationBuilder builder, out JwtSettings jwtSettings)
        {
            builder.Services.AddMemoryCache(options => builder.Configuration.GetSection("MemoryCache").Bind(options));
            var jwtSection = builder.Configuration.GetSection(JwtSettings.Section);
            builder.Services
                   .Configure<JwtSettings>(jwtSection);

            jwtSettings = new JwtSettings()
            {
                Audience = "",
                Expiration = 0,
                Issuer = "",
                Secret = ""
            };
            jwtSection.Bind(jwtSettings);
        }
        internal static void ConfigurePortalServices(this WebApplicationBuilder builder)
        {
            builder.Services
                   .AddSingleton<ICacheService, CacheService>()
                   .AddScoped<IUserService, UserService>()
                   .AddHttpContextAccessor()
                   .AddSignalR();
        }
    }
}
