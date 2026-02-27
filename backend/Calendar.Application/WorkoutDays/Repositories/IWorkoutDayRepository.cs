using Calendar.Domain.Calendars.Aggregates;
using Calendar.Shared.Errors;
using CSharpFunctionalExtensions;

namespace Calendar.Application.WorkoutDays.Repositories;

public interface IWorkoutDayRepository
{
    Task<Result<Guid, Error>> Add(WorkoutDay workoutDay, CancellationToken cancellationToken);
    
    Task<Result<WorkoutDay, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
  
    /*Task<IReadOnlyList<WorkoutDay>> GetByDateRangeAsync(DateOnly from, DateOnly to,
        CancellationToken cancellationToken);*/
    
    Task<Result<WorkoutDay, Error>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken);
}