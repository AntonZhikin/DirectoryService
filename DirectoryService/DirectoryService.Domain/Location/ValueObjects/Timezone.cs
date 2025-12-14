using CSharpFunctionalExtensions;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Domain.Location;

public record Timezone
{
    public string Value { get; }

    private Timezone(string value)
    {
        Value = value;
    }

    public static Result<Timezone, AppError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return AppErrors.ValueIsInvalid(value);

        bool isValid = TimeZoneInfo.TryFindSystemTimeZoneById(value, out _);

        if (!isValid)
            return AppErrors.ValueIsInvalid(value);

        return new Timezone(value);
    }
}