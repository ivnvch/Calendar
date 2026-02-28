using Calendar.Application.Abstractions.Database;
using Calendar.Application.CQRS;
using Calendar.Shared.DTOs;
using Calendar.Shared.Errors;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Application.WorkoutDays.Queries.GetMonthPeriodHandler;

public class GetMonthPeriodHandler : IQueryHandler<IReadOnlyList<MonthDayPeriodDto>, GetMonthPeriodQuery>
{
    private readonly IReadDbContext _readDbContext;

    public GetMonthPeriodHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<IReadOnlyList<MonthDayPeriodDto>, Errors>> Handle(
        GetMonthPeriodQuery query, CancellationToken cancellationToken)
    {
        var from = new DateOnly(query.Year, query.Month, 1);
        var to = from.AddMonths(1).AddDays(-1);

        var period = await _readDbContext.WorkoutDaysRead
            .Where(w => w.Date >= from && w.Date <= to)
            .OrderBy(w => w.Date)
            .Select(w => new MonthDayPeriodDto(
                w.Date,
                w.Exercises.Count))
            .ToListAsync(cancellationToken);

        return period;
    }
}