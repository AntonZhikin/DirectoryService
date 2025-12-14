using CSharpFunctionalExtensions;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Domain.Location;

public record LocationName
{
    public const int NAME_MIN_LENGHT = 3;
    public const int NAME_MAX_LENGHT = 120;

    public string Value { get; }

    private LocationName(string value)
    {
        Value = value;
    }

    public static Result<LocationName, AppError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length is < NAME_MIN_LENGHT or > NAME_MAX_LENGHT)
        {
            return AppErrors.ValueIsInvalid(value);
        }

        return new LocationName(value);
    }
}