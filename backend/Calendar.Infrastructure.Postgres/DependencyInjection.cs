using Calendar.Application.Abstractions.Database;
using Calendar.Application.WorkoutDays.Repositories;
using Calendar.Infrastructure.Postgres.Database;
using Calendar.Infrastructure.Postgres.WorkoutDays;
using Calendar.Infrastructure.Postgres.WorkoutDays.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Calendar.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddRepositories()
            .AddPostgres(configuration);

        return services;
    }

    private static IServiceCollection AddPostgres(
        this IServiceCollection services, IConfiguration configuration)
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

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IWorkoutDayRepository, WorkoutDayRepository>();
        services.AddScoped<ITransactionManager, TransactionManager>();

        return services;
    }
}