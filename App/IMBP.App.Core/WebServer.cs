using IMBP.App.Core.Services;
using IMBP.App.Domain.Specifications;
using IMBP.App.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IMBP.App.Core
{
    public class WebServer
    {
        public static void Run(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
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

        }
        internal static void ConfigurePortalServices(this WebApplicationBuilder builder)
        {
            builder.Services
                   .AddScoped<IUserService, UserService>();
        }
    }
}
