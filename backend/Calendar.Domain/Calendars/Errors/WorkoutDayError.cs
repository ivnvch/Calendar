using System.Runtime.InteropServices.JavaScript;
using Calendar.Shared.Errors;

namespace Calendar.Domain.Calendars.Errors;

public static class WorkoutDayError
{
    public static Error DatabaseError() =>
        Error.Failure(new ErrorMessage("workoutDay.database.error", 
            "The error occurred while working with the database"));
    
    public static Error OperationCancelled() =>
        Error.Failure(new ErrorMessage("workoutDay.operation.cancelled", "Operation was cancelled"));
    
    public static Error WorkoutDayConflict(DateOnly date) =>
        Error.Conflict("workoutDay.conflict.date",
            $"WorkoutDay with this date: {date} already exists");
}