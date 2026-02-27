using Calendar.Application.WorkoutDays.Repositories;
using Calendar.Domain.Calendars.Aggregates;
using Calendar.Domain.Calendars.Errors;
using Calendar.Shared.Errors;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Calendar.Infrastructure.Postgres.WorkoutDays.Repositories;

public class WorkoutDayRepository : IWorkoutDayRepository
{
    private readonly CalendarDbContext _context;
    private readonly ILogger<WorkoutDayRepository> _logger;

    public WorkoutDayRepository(CalendarDbContext context, ILogger<WorkoutDayRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> Add(WorkoutDay workoutDay, CancellationToken cancellationToken)
    {
        try
        {
            _context.WorkoutDays.Add(workoutDay);
            _logger.LogInformation($"Added WorkoutDay with ID: {workoutDay.Id}");

            return workoutDay.Id;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            if (pgEx is { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: not null } &&
                pgEx.ConstraintName.Contains("idx_workout_days_date", StringComparison.InvariantCultureIgnoreCase))
            {
                return WorkoutDayError.WorkoutDayConflict(workoutDay.Date);
            }
            
            _logger.LogError(ex, "An error occurred while adding workoutDay: {workoutDay}", workoutDay);

            return WorkoutDayError.DatabaseError();
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation cancelled while creating workoutDay: {workoutDay}", workoutDay);

            return WorkoutDayError.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occured while adding workoutDay: {workoutDay}",  workoutDay);

            return WorkoutDayError.DatabaseError();
        }
    }

    public async Task<Result<WorkoutDay, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = await _context.WorkoutDays
            .Include(w => w.Exercises)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        if (query is null)
            return GeneralErrors.NotFound(id);
        
        return query;
    }

    public async Task<Result<WorkoutDay, Error>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken)
    {
        var query = await _context.WorkoutDays
            .Include(w => w.Exercises)
            .FirstOrDefaultAsync(w => w.Date == date, cancellationToken);

        if (query is null)
            return GeneralErrors.NotFound(date);
        
        return query;
    }
}