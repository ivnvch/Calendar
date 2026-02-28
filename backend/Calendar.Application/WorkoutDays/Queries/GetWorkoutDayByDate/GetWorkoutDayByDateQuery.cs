using Calendar.Application.CQRS;

namespace Calendar.Application.WorkoutDays.Queries.GetWorkoutDayByDate;

public record GetWorkoutDayByDateQuery(DateOnly Date) : IQuery;