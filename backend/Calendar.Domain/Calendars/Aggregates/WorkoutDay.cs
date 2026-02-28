using Calendar.Domain.Calendars.Enums;
using Calendar.Shared.Errors;
using CSharpFunctionalExtensions;

namespace Calendar.Domain.Calendars.Aggregates;

public sealed class WorkoutDay : Common.Entity<Guid>
{
    private readonly List<Exercise> _exercises = [];

    public DateOnly Date { get; private set; }
    public IReadOnlyCollection<Exercise> Exercises => _exercises.AsReadOnly();

    private WorkoutDay() { }

    public WorkoutDay(DateOnly date) : base(Guid.NewGuid())
    {
        Date = date;
    }

    public Result<Exercise, Error> AddExercise(ActivityType activityType, decimal targetValue)
    {
        if (targetValue <= 0)
            return GeneralErrors.ValueIsRequired(nameof(targetValue));
        
        var exercise = new Exercise(activityType, targetValue);
        _exercises.Add(exercise);
        
        return exercise;
    }

    public UnitResult<Error> UpdateExerciseStatus(Guid exerciseId, ExerciseStatus newStatus)
    {
        Result<Exercise, Error> exercise = GetExerciseOrError(exerciseId);
        if (exercise.IsFailure)
            return UnitResult.Failure(exercise.Error);
        
        UnitResult<Error> changeStatus = exercise.Value.ChangeStatus(newStatus);

        return changeStatus;
    }

    public UnitResult<Error> UpdateExerciseProgress(Guid exerciseId, decimal actualValue)
    {
        Result<Exercise, Error> exercise = GetExerciseOrError(exerciseId);
        
        if (exercise.IsFailure)
            return UnitResult.Failure(exercise.Error);
        
        UnitResult<Error> updateProgress = exercise.Value.UpdateProgress(actualValue);

        return updateProgress;
    }

    private Result<Exercise, Error> GetExerciseOrError(Guid exerciseId)
    {
        var result = _exercises.Find(e => e.Id == exerciseId);
        
        if (result is null)
            return GeneralErrors.NotFound(exerciseId);
        
        return result;
        
    }
}
