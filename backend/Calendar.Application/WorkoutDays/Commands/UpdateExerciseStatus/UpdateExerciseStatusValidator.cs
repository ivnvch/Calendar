using Calendar.Domain.Calendars.Enums;
using FluentValidation;

namespace Calendar.Application.WorkoutDays.Commands.UpdateExerciseStatus;

public class UpdateExerciseStatusValidator : AbstractValidator<UpdateExerciseStatusCommand>
{
    public UpdateExerciseStatusValidator()
    {
        RuleFor(c => c.ExerciseId)
            .NotEmpty()
            .WithMessage("Exercise ID is required");

        RuleFor(c => c.Status)
            .NotEmpty()
            .Must(s => Enum.TryParse<ExerciseStatus>(s, ignoreCase: true, out _))
            .WithMessage("Invalid exercise status");
    }
}