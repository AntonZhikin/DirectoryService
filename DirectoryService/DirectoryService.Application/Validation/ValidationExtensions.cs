using System.Text.Json;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation.Results;

namespace DirectoryService.Application.Validation;

public static class ValidationExtensions
{
    public static AppError ToAppError(this ValidationResult validationResult)
    {
        List<ValidationFailure> validationErrors = validationResult.Errors;

        IEnumerable<IReadOnlyList<AppErrorMessage>> errors =
            from validationError in validationErrors
            let errorMessage = validationError.ErrorMessage
            let error = JsonSerializer.Deserialize<AppError>(errorMessage)
            select error.Messages;
        
        return AppError.Validation(errors.SelectMany(e => e));
    }
}
