using DirectoryService.Shared.ErrorManagement;
using FluentValidation.Results;

namespace DirectoryService.Application.Validation;

public static class ValidationExtensions
{
    public static AppError ToAppError(this ValidationResult validationResult)
    {
        var messages = validationResult.Errors
            .Select(e => AppError.DeserializeToMessage(e.ErrorMessage, e.PropertyName))
            .ToArray();

        return AppError.Validation(messages);
    }
}
