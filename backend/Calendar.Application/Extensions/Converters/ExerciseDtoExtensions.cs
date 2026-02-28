using Calendar.Domain.Calendars.Enums;
using Calendar.Shared.DTOs.Exercises;

namespace Calendar.Application.Extensions.Converters;

public static class EnumExtensions
{
    public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct, Enum
        => Enum.Parse<TEnum>(value, ignoreCase: true);
}