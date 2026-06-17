using System.Text.Json;
using CSharpFunctionalExtensions;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;

namespace DirectoryService.Application.Validation;


public static class CustomValidators
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement, TValueObject>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, Result<TValueObject, AppError>> factoryMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            Result<TValueObject, AppError> result = factoryMethod(value);

            if (result.IsSuccess)
                return;

            context.AddFailure(JsonSerializer.Serialize(result.Error));
        });
    }

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, AppError error)
    {
        return rule.WithMessage(JsonSerializer.Serialize(error));
    }
}