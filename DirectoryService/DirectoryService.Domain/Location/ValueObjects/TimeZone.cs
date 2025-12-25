using CSharpFunctionalExtensions;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Domain.Location.ValueObjects;

public record TimeZone
{
    public string Value { get; }

    private TimeZone(string value)
    {
        Value = value;
    }

    public static Result<TimeZone, AppError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return AppErrors.ValueIsInvalid(value);

        bool isValid = TimeZoneInfo.TryFindSystemTimeZoneById(value, out _);

        if (!isValid)
            return AppErrors.ValueIsInvalid(value);

        return new TimeZone(value);
    }
}