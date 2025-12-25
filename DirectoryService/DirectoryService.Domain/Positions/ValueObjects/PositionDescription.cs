using CSharpFunctionalExtensions;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Domain.Positions.ValueObjects;

public record PositionDescription
{
    public const int MAX_LENGTH_POSITION_DESCRIPTION = 100;
    public string Value { get; }

    public PositionDescription(string value)
    {
        Value = value;
    }

    public static Result<PositionDescription, AppError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > MAX_LENGTH_POSITION_DESCRIPTION)
            return AppErrors.ValueIsInvalid("PositionDescription");
        
        return new PositionDescription(value);
    }
}