using Calendar.Domain.Calendars.Enums;
using FluentValidation;

namespace Calendar.Application.WorkoutDays.Commands.CreateWorkoutDay;

public class CreateWorkoutDayValidator : AbstractValidator<CreateWorkoutDayCommand>
{
    public CreateWorkoutDayValidator()
    {
        RuleFor(command => command.Date)
            .NotNull()
            .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("The date cannot be in the past");
        
        RuleFor(command => command.ExerciseDtos)
            .NotNull()
            .NotEmpty()
            .WithMessage("At least one exercise is required");

        RuleForEach(command => command.ExerciseDtos)
            .ChildRules(exercise =>
            {
                exercise.RuleFor(e => e.ActivityType)
                    .NotNull()
                    .Must(activityType => Enum.TryParse<ActivityType>(activityType, ignoreCase: true, out _))
                    .WithMessage("The activity type is not recognized");
                
                exercise.RuleFor(e => e.TargetValue)
                    .GreaterThan(0)
                    .WithMessage("Target value must be greater than zero");
            });
    }
}