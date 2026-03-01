using FluentValidation;

namespace Calendar.Application.WorkoutDays.Queries.GetMonthPeriodHandler;

public class GetMonthPeriodValidator : AbstractValidator<GetMonthPeriodQuery>
{
    public GetMonthPeriodValidator()
    {
        RuleFor(q => q.Year)
            .InclusiveBetween(1, 9999)
            .WithMessage("Year must be between 1 and 9999");

        RuleFor(q => q.Month)
            .InclusiveBetween(1, 12)
            .WithMessage("Month must be between 1 and 12");
    }
}
