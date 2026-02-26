using Calendar.Domain.Calendars.Enums;
using Calendar.Domain.Common;
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
        
        exercise.Value.ChangeStatus(newStatus);
        
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> UpdateExerciseProgress(Guid exerciseId, decimal actualValue)
    {
        Result<Exercise, Error> exercise = GetExerciseOrError(exerciseId);
        
        if (exercise.IsFailure)
            return UnitResult.Failure(exercise.Error);
        
        exercise.Value.UpdateProgress(actualValue);
        
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> RemoveExercise(Guid exerciseId)
    {
        Result<Exercise, Error> exercise = GetExerciseOrError(exerciseId);
        if (exercise.IsFailure)
            return UnitResult.Failure(exercise.Error);
        
        _exercises.Remove(exercise.Value);
        
        return UnitResult.Success<Error>();
    }

    private Result<Exercise, Error> GetExerciseOrError(Guid exerciseId)
    {
        var result = _exercises.Find(e => e!.Id == exerciseId);
        
        if (result is null)
            return GeneralErrors.NotFound(exerciseId);
        
        return result;
        
    }
}
