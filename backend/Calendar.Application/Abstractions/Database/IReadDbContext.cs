using Calendar.Domain.Calendars.Aggregates;

namespace Calendar.Application.Abstractions.Database;

public interface IReadDbContext
{
    IQueryable<WorkoutDay> WorkoutDaysRead { get; }
}