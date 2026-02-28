using Calendar.API.Extensions;
using Calendar.API.Models.Requests;
using Calendar.Application.CQRS;
using Calendar.Application.WorkoutDays.Commands.CreateWorkoutDay;
using Calendar.Application.WorkoutDays.Commands.UpdateExerciseStatus;
using Calendar.Application.WorkoutDays.Queries.GetMonthPeriodHandler;
using Calendar.Application.WorkoutDays.Queries.GetWorkoutDayByDate;
using Calendar.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.API.Controllers;

[ApiController]
[Route("api/workout-days")]
public class WorkoutDaysController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateWorkoutDay(
        [FromBody] CreateWorkoutDayRequest request,
        [FromServices] ICommandHandler<Guid, CreateWorkoutDayCommand> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request.Command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpGet("{date}")]
    public async Task<ActionResult<WorkoutDayDetailsDto>> GetByDate(
        [FromRoute] DateOnly date,
        [FromServices] IQueryHandler<WorkoutDayDetailsDto, GetWorkoutDayByDateQuery> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetWorkoutDayByDateQuery(date);
        var result = await handler.Handle(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpGet("period")]
    public async Task<ActionResult<IReadOnlyList<MonthDayPeriodDto>>> GetMonthPeriod(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromServices] IQueryHandler<IReadOnlyList<MonthDayPeriodDto>, GetMonthPeriodQuery> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetMonthPeriodQuery(year, month);
        var result = await handler.Handle(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPatch("{date}/exercises/{exerciseId:guid}/status")]
    public async Task<ActionResult> UpdateExerciseStatus(
        [FromRoute] DateOnly date,
        [FromRoute] Guid exerciseId,
        [FromBody] UpdateExerciseStatusRequest request,
        [FromServices] ICommandHandler<UpdateExerciseStatusCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateExerciseStatusCommand(date, exerciseId, request.Status);
        var result = await handler.Handle(command, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : NoContent();
    }
}