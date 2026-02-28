using Calendar.Application.Abstractions.Database;
using Calendar.Application.CQRS;
using Calendar.Domain.Calendars;
using Calendar.Shared.DTOs;
using Calendar.Shared.Errors;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Application.WorkoutDays.Queries.GetWorkoutDayByDate;

public class GetWorkoutDayByDateHandler : IQueryHandler<WorkoutDayDetailsDto, GetWorkoutDayByDateQuery>
{
    private readonly IReadDbContext _readDbContext;

    public GetWorkoutDayByDateHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<WorkoutDayDetailsDto, Errors>> Handle(GetWorkoutDayByDateQuery query,
        CancellationToken cancellationToken)
    {
        var workoutDay = await _readDbContext.WorkoutDaysRead
            .Where(w => w.Date == query.Date)
            .Select(w => new WorkoutDayDetailsDto(
                w.Id,
                w.Date,
                w.Exercises.Select(e => new ExerciseDetailsDto(
                    e.Id,
                    e.ActivityType.ToString(),
                    e.Status.ToString(),
                    e.TargetValue,
                    e.ActualValue,
                    e.CreatedAt
                )).ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        if (workoutDay is null)
            return GeneralErrors.NotFound(query.Date).ToErrors();

        return workoutDay;
    }
}