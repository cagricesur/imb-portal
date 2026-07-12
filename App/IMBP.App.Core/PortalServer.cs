using IMBP.App.Core.Services;
using IMBP.App.Domain.Settings;
using IMBP.App.Domain.Specifications;
using IMBP.App.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
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

            builder.ConfigurePortalSettings();
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
        }
        internal static void ConfigurePortalServices(this WebApplicationBuilder builder)
        {
            builder.Services
                   .AddSingleton<ICacheService, CacheService>()
                   .AddScoped<ITranslationService, TranslationService>()
                   .AddScoped<IUserService, UserService>();
        }
    }
}
