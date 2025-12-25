using DirectoryService.Shared.ErrorManagement;
using FluentValidation.Results;

namespace DirectoryService.Application.Validation;

public static class ValidationExtensions
{
    public static AppErrorList ToList(this ValidationResult validationResult)
    {
        var validationErrors = validationResult.Errors;

        var errors = from validationError in validationErrors
            let errorMessage = validationError.ErrorMessage
            let error = AppError.Deserialize(errorMessage)
            select AppError.Validation(error.Code, error.Message, validationError.PropertyName);

        return errors.ToList();
    }
}