using Calendar.Domain.Calendars.Enums;
using Calendar.Domain.Common;

namespace Calendar.Domain.Calendars;

public sealed class Exercise : Entity<Guid>
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
    public string? Notes { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Exercise() { }

    internal Exercise(ActivityType activityType, decimal targetValue, string? notes)
        : base(Guid.NewGuid())
    {
        if (targetValue <= 0)
            throw new DomainException("Target value must be greater than zero.");

        ActivityType = activityType;
        Status = ExerciseStatus.Planned;
        TargetValue = targetValue;
        ActualValue = 0;
        Notes = notes;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeStatus(ExerciseStatus newStatus)
    {
        if (Status == newStatus)
            return;

        if (!AllowedTransitions[Status].Contains(newStatus))
            throw new DomainException(
                $"Cannot transition from '{Status}' to '{newStatus}'.");

        Status = newStatus;
    }

    public void UpdateProgress(decimal actualValue)
    {
        if (actualValue < 0)
            throw new DomainException("Actual value cannot be negative.");

        if (Status is ExerciseStatus.Completed or ExerciseStatus.Skipped)
            throw new DomainException(
                $"Cannot update progress when exercise is '{Status}'.");

        ActualValue = actualValue;
    }
}
