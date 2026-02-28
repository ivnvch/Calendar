using Calendar.Application.CQRS;

namespace Calendar.Application.WorkoutDays.Queries.GetMonthPeriodHandler;

public record GetMonthPeriodQuery(int Year, int Month) : IQuery;