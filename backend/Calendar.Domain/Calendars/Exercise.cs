using Calendar.Domain.Calendars.Enums;
using Calendar.Shared.Errors;
using CSharpFunctionalExtensions;

namespace Calendar.Domain.Calendars;

public sealed class Exercise : Common.Entity<Guid>
{
    private static readonly Dictionary<ExerciseStatus, HashSet<ExerciseStatus>> AllowedTransitions = new()
    {
        [ExerciseStatus.Planned] = [ExerciseStatus.InProgress, ExerciseStatus.Completed, ExerciseStatus.Skipped],
        [ExerciseStatus.InProgress] = [ExerciseStatus.Completed, ExerciseStatus.Skipped],
        [ExerciseStatus.Completed] = [],
        [ExerciseStatus.Skipped] = []
    };

    public ActivityType ActivityType { get; private set; }
    public ExerciseStatus Status { get; private set; }
    public decimal TargetValue { get; private set; }
    public decimal ActualValue { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Exercise() { }

    internal Exercise(ActivityType activityType, decimal targetValue)
        : base(Guid.NewGuid())
    {
        ActivityType = activityType;
        Status = ExerciseStatus.Planned;
        TargetValue = targetValue;
        ActualValue = 0;
        CreatedAt = DateTime.UtcNow;
    }

    public UnitResult<Error> ChangeStatus(ExerciseStatus newStatus)
    {
        if (Status == newStatus)
            return UnitResult.Success<Error>();

        if (!AllowedTransitions[Status].Contains(newStatus))
            return UnitResult.Failure<Error>(Error.Failure("change.status.failed", $"Cannot transition from '{Status}' to '{newStatus}'."));

        Status = newStatus;
        
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> UpdateProgress(decimal actualValue)
    {
        if (actualValue < 0)
            return GeneralErrors.ValueIsInvalid(nameof(actualValue));

        if (Status is ExerciseStatus.Completed or ExerciseStatus.Skipped)
            return UnitResult.Failure<Error>(Error.Validation(new ErrorMessage("progress.can.not.updated", $"Cannot update progress when exercise is '{Status}'")));

        ActualValue = actualValue;
        
        return UnitResult.Success<Error>();
    }
}
