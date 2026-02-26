using System.Reflection;
using Calendar.Domain.Calendars;
using Calendar.Domain.Calendars.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Infrastructure.Postgres;

public class CalendarDbContext : DbContext
{
    public DbSet<Exercise> Exercises { get; set; } = null!;
    public DbSet<WorkoutDay> WorkoutDays { get; set; } = null!;

    public CalendarDbContext(DbContextOptions<CalendarDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}