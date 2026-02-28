using Calendar.Application.Abstractions.Database;
using Calendar.Application.CQRS;
using Calendar.Application.Extensions.Converters;
using Calendar.Application.Extensions.Validations;
using Calendar.Application.WorkoutDays.Repositories;
using Calendar.Domain.Calendars.Aggregates;
using Calendar.Domain.Calendars.Enums;
using Calendar.Shared.Errors;
using CSharpFunctionalExtensions;
using FluentValidation;

namespace Calendar.Application.WorkoutDays.Commands.UpdateExerciseStatus;

public class UpdateExerciseStatusHandler : ICommandHandler<UpdateExerciseStatusCommand>
{
    private readonly ITransactionManager _transactionManager;
    private readonly IWorkoutDayRepository _workoutDayRepository;
    private readonly IValidator<UpdateExerciseStatusCommand> _validator;

    public UpdateExerciseStatusHandler(
        ITransactionManager transactionManager,
        IWorkoutDayRepository workoutDayRepository, 
        IValidator<UpdateExerciseStatusCommand> validator)
    {
        _transactionManager = transactionManager;
        _workoutDayRepository = workoutDayRepository;
        _validator = validator;
    }

    public async Task<UnitResult<Errors>> Handle(UpdateExerciseStatusCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.Errors.ToErrors();

        var transactionScopeResult = await _transactionManager.BeginTransaction(cancellationToken);
        
        using var transactionScope = transactionScopeResult.Value;
        
        Result<WorkoutDay, Error> dayResult = await _workoutDayRepository
            .GetByDateAsync(command.Date, cancellationToken);
        
        if (dayResult.IsFailure)
        {
            transactionScope.Rollback(cancellationToken);
            return dayResult.Error.ToErrors();
        }
        
        var statusResult = dayResult.Value.UpdateExerciseStatus(command.ExerciseId, command.Status.ToEnum<ExerciseStatus>());
        if (statusResult.IsFailure)
        {
            transactionScope.Rollback(cancellationToken);
            return statusResult.Error.ToErrors();
        }

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return UnitResult.Failure(saveResult.Error.ToErrors());
        
        var commitedResult =  transactionScope.Commit(cancellationToken);
        if (commitedResult.IsFailure)
            return commitedResult.Error.ToErrors();
        
        return UnitResult.Success<Errors>();
    }
}