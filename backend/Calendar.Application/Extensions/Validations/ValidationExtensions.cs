using Calendar.Shared.Errors;
using FluentValidation.Results;

namespace Calendar.Application.Extensions.Validations;

public static class ValidationExtensions
{
    public static Errors ToErrors(this List<ValidationFailure> validationResult)
    {
        var errors = validationResult
            .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage, e.PropertyName))
            .ToArray();

        return errors;
    }
}