using Calendar.Application.CQRS;
using Calendar.Application.Extensions.Converters;
using Calendar.Application.Extensions.Validations;
using Calendar.Application.WorkoutDays.Repositories;
using Calendar.Domain.Calendars;
using Calendar.Domain.Calendars.Aggregates;
using Calendar.Domain.Calendars.Enums;
using Calendar.Shared.Errors;
using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;

namespace Calendar.Application.WorkoutDays.Commands.CreateWorkoutDay;

public sealed class CreateWorkoutDayHandler : ICommandHandler<Guid, CreateWorkoutDayCommand>
{
    private readonly IWorkoutDayRepository _workoutDayRepository;
    private readonly IValidator<CreateWorkoutDayCommand> _validator;

    public CreateWorkoutDayHandler(IWorkoutDayRepository workoutDayRepository, IValidator<CreateWorkoutDayCommand> validator)
    {
        _workoutDayRepository = workoutDayRepository;
        _validator = validator;
    }

    public async Task<Result<Guid, Errors>> Handle(CreateWorkoutDayCommand command, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors();
        }

        WorkoutDay day = new WorkoutDay(command.Date);

        foreach (var exerciseResult in command.Exercises.Select(e => day.AddExercise(e.ActivityType.ToEnum<ActivityType>(), e.TargetValue)))
        {
            if (exerciseResult.IsFailure)
                return exerciseResult.Error.ToErrors();
        }

        Result<Guid, Error> addedWorkoutDay = await _workoutDayRepository.Add(day, cancellationToken);
        
        if (addedWorkoutDay.IsFailure)
            return addedWorkoutDay.Error.ToErrors();
        
        return addedWorkoutDay.Value;
    }
}