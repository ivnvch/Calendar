using Calendar.Application.CQRS;
using Calendar.Application.Extensions.Converters;
using Calendar.Application.Extensions.Validations;
using Calendar.Application.WorkoutDays.Repositories;
using Calendar.Domain.Calendars;
using Calendar.Domain.Calendars.Aggregates;
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

        IEnumerable<Result<Exercise,Error>> exercises = command.ExerciseDtos.Select(e => day.AddExercise(e.ToActivityType(), e.TargetValue));
        
        Result<Exercise, Error> failed = exercises.FirstOrDefault(r => r.IsFailure);
        if (failed.IsFailure)
            return failed.Error.ToErrors();

        Result<Guid, Error> addedWorkoutDay = await _workoutDayRepository.Add(day, cancellationToken);
        
        if (addedWorkoutDay.IsFailure)
            return addedWorkoutDay.Error.ToErrors();
        
        return addedWorkoutDay.Value;
    }
}