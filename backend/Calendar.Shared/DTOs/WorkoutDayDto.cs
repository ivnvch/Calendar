namespace Calendar.Shared.DTOs;

public record WorkoutDayDetailsDto(
    Guid Id,
    DateOnly Date,
    IReadOnlyList<ExerciseDetailsDto> Exercises);
    
public record ExerciseDetailsDto(
    Guid Id,
    string ActivityType,
    string Status,
    decimal TargetValue,
    decimal ActualValue,
    DateTime CreatedAt);