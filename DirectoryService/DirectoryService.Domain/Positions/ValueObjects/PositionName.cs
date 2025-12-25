using CSharpFunctionalExtensions;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Domain.Positions.ValueObjects;

public record PositionName
{
    public const int MAX_LENGTH_POSITION_NAME = 100;
    public string Value { get; }

    public PositionName(string value)
    {
        Value = value;
    }

    public static Result<PositionName, AppError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > MAX_LENGTH_POSITION_NAME)
            return AppErrors.ValueIsInvalid("PositionName");
        
        return new PositionName(value);
    }
}