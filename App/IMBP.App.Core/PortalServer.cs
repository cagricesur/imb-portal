using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using IMBP.App.Core.Services;
using IMBP.App.Domain.Settings;
using IMBP.App.Domain.Specifications;
using IMBP.App.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

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

            builder.ConfigurePortalSettings();
            builder.ConfigurePortalContext();
            builder.ConfigurePortalAuthentication();
            builder.ConfigurePortalRateLimiting();
            builder.ConfigurePortalServices();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.MapStaticAssets();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
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

        internal static void ConfigurePortalSettings(this WebApplicationBuilder builder)
        {
            builder.Services.AddMemoryCache(options => builder.Configuration.GetSection("MemoryCache").Bind(options));
            builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection(ApplicationSettings.Section));
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.Section));
            builder.Services.Configure<ActiveDirectorySettings>(builder.Configuration.GetSection(ActiveDirectorySettings.Section));
        }

        internal static void ConfigurePortalAuthentication(this WebApplicationBuilder builder)
        {
            var jwtSettings = builder.Configuration.GetSection(JwtSettings.Section).Get<JwtSettings>()
                ?? throw new InvalidOperationException($"{JwtSettings.Section} configuration is missing.");

            if (string.IsNullOrWhiteSpace(jwtSettings.Secret) || jwtSettings.Secret.Length < 32)
            {
                throw new InvalidOperationException($"{JwtSettings.Section}:Secret must be at least 32 characters.");
            }

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        ClockSkew = TimeSpan.FromMinutes(1),
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (string.IsNullOrEmpty(context.Token)
                                && context.Request.Cookies.TryGetValue(jwtSettings.CookieName, out var cookieToken))
                            {
                                context.Token = cookieToken;
                            }

                            return Task.CompletedTask;
                        },
                    };
                });

            builder.Services.AddAuthorization();
        }

        internal static void ConfigurePortalRateLimiting(this WebApplicationBuilder builder)
        {
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddPolicy("authenticate", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                        }));
            });
        }

        internal static void ConfigurePortalServices(this WebApplicationBuilder builder)
        {
            builder.Services
                   .AddSingleton<ICacheService, CacheService>()
                   .AddScoped<ITranslationService, TranslationService>()
                   .AddScoped<IUserService, UserService>()
                   .AddScoped<ITokenService, TokenService>();

#pragma warning disable CA1416 // Active Directory APIs are Windows-only by design
            builder.Services.AddScoped<IActiveDirectoryService, ActiveDirectoryService>();
#pragma warning restore CA1416
        }
    }
}
