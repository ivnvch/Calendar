using Calendar.Application.Abstractions.Database;
using Calendar.Application.WorkoutDays.Repositories;
using Calendar.Infrastructure.Postgres.WorkoutDays;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Calendar.Infrastructure.Postgres;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services
                .AddRepositories()
                .AddPostgres(configuration);
        
            return services;
        }

        private IServiceCollection AddPostgres(IConfiguration configuration)
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
            
            services.AddScoped<IReadDbContext>(provider => provider.GetRequiredService<CalendarDbContext>());
            
            return services;
        }

        private IServiceCollection AddRepositories()
        {
            services.AddScoped<IWorkoutDayRepository, WorkoutDayRepository>();
        
            return services;
        }

    }
}