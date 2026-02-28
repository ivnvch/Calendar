using Calendar.Shared.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.API.Extensions;

public static class ErrorExtensions
{
    public static ActionResult ToResponse(this Errors errors)
    {
        var statusCode = GetStatusCode(errors);
        var envelope = Envelope.Error(errors);

        return new ObjectResult(envelope) { StatusCode = statusCode };
    }

    private static int GetStatusCode(Errors errors)
    {
        var errorTypes = errors.Select(e => e.Type).Distinct().ToList();

        if (errorTypes.Count > 1)
            return StatusCodes.Status500InternalServerError;

        return errorTypes.First() switch
        {
            ErrorType.VALIDATION => StatusCodes.Status400BadRequest,
            ErrorType.NOT_FOUND => StatusCodes.Status404NotFound,
            ErrorType.CONFLICT => StatusCodes.Status409Conflict,
            ErrorType.FAILURE => StatusCodes.Status500InternalServerError,
            ErrorType.AUTHENTICATION => StatusCodes.Status401Unauthorized,
            ErrorType.AUTHORIZATION => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}