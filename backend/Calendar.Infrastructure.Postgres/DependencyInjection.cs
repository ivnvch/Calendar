using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Calendar.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgres(configuration);
        
        return services;
    }

    private static void AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<CalendarDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("calendar_connection");

            IHostEnvironment? hostEnvironment = sp.GetService<IHostEnvironment>();
            ILoggerFactory? loggerFactory = sp.GetService<ILoggerFactory>();
            
            options.UseNpgsql(connectionString);

            if (hostEnvironment == null || !hostEnvironment.IsDevelopment()) return;
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            
            options.UseLoggerFactory(loggerFactory);

        });
    }
}