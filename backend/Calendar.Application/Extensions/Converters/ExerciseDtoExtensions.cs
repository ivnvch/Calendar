using Calendar.Domain.Calendars.Enums;
using Calendar.Shared.DTOs.Exercises;

namespace Calendar.Application.Extensions.Converters;

public static class ExerciseDtoExtensions
{
    public static ActivityType ToActivityType(this ExerciseDto dto)
        => Enum.Parse<ActivityType>(dto.ActivityType, ignoreCase: true);
}