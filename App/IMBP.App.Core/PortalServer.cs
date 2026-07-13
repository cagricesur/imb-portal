using IMBP.App.Core.Services;
using IMBP.App.Domain.Settings;
using IMBP.App.Domain.Specifications;
using IMBP.App.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
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
            builder.Services.AddHttpContextAccessor();

            builder.ConfigurePortalSettings();
            builder.ConfigurePortalAuthentication();
            builder.ConfigurePortalContext();
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
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.Section));
            builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection(AuthenticationSettings.Section));
        }

        internal static void ConfigurePortalAuthentication(this WebApplicationBuilder builder)
        {
            var jwtSettings = builder.Configuration.GetSection(JwtSettings.Section).Get<JwtSettings>()
                ?? throw new InvalidOperationException("JwtSettings configuration is required.");

            if (string.IsNullOrWhiteSpace(jwtSettings.Secret))
            {
                throw new InvalidOperationException("JwtSettings:Secret must be configured.");
            }

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        ClockSkew = TimeSpan.FromMinutes(1),
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies[jwtSettings.CookieNames.AccessToken];
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = async context =>
                        {
                            var sessionIdValue = context.Principal?.FindFirst("sid")?.Value;
                            if (!Guid.TryParse(sessionIdValue, out var sessionId))
                            {
                                context.Fail("Invalid session identifier.");
                                return;
                            }

                            var sessionService = context.HttpContext.RequestServices
                                .GetRequiredService<ISessionService>();
                            var userAgent = context.HttpContext.Request.Headers.UserAgent.ToString();
                            var isValid = await sessionService.IsSessionValidAsync(sessionId, userAgent);

                            if (!isValid)
                            {
                                context.Fail("Session is no longer valid.");
                            }
                        },
                    };
                });

            builder.Services.AddAuthorization();
        }

        internal static void ConfigurePortalServices(this WebApplicationBuilder builder)
        {
            builder.Services
                   .AddSingleton<ICacheService, CacheService>()
                   .AddScoped<ITranslationService, TranslationService>()
                   .AddScoped<IUserService, UserService>()
                   .AddScoped<ITokenService, TokenService>()
                   .AddScoped<ISessionService, SessionService>()
                   .AddScoped<ICookieAuthService, CookieAuthService>();
        }
    }
}
