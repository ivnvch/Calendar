using Calendar.Domain.Calendars.Enums;
using Calendar.Domain.Common;

namespace Calendar.Domain.Calendars.Aggregates;

public sealed class WorkoutDay : Entity<Guid>
{
    private readonly List<Exercise> _exercises = [];

    public DateOnly Date { get; private set; }
    public IReadOnlyCollection<Exercise> Exercises => _exercises.AsReadOnly();

    private WorkoutDay() { }

    public WorkoutDay(DateOnly date) : base(Guid.NewGuid())
    {
        Date = date;
    }

    public Exercise AddExercise(ActivityType activityType, decimal targetValue, string? notes = null)
    {
        var exercise = new Exercise(activityType, targetValue, notes);
        _exercises.Add(exercise);
        return exercise;
    }

    public void UpdateExerciseStatus(Guid exerciseId, ExerciseStatus newStatus)
    {
        var exercise = GetExerciseOrThrow(exerciseId);
        exercise.ChangeStatus(newStatus);
    }

    public void UpdateExerciseProgress(Guid exerciseId, decimal actualValue)
    {
        var exercise = GetExerciseOrThrow(exerciseId);
        exercise.UpdateProgress(actualValue);
    }

    public void RemoveExercise(Guid exerciseId)
    {
        var exercise = GetExerciseOrThrow(exerciseId);
        _exercises.Remove(exercise);
    }

    private Exercise GetExerciseOrThrow(Guid exerciseId)
    {
        return _exercises.Find(e => e.Id == exerciseId)
               ?? throw new DomainException($"Exercise with id '{exerciseId}' not found.");
    }
}
