using Calendar.Application.CQRS;

namespace Calendar.Application.WorkoutDays.Commands.UpdateExerciseStatus;

public record UpdateExerciseStatusCommand(DateOnly Date, Guid ExerciseId, string Status) : ICommand;