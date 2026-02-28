using Calendar.Application.CQRS;
using Calendar.Shared.DTOs.Exercises;

namespace Calendar.Application.WorkoutDays.Commands.CreateWorkoutDay;

public record CreateWorkoutDayCommand(DateOnly Date, List<CreateExerciseDto> Exercises) : ICommand;