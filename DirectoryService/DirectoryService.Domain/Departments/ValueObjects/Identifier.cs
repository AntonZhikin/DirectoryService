using CSharpFunctionalExtensions;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Domain.Departments.ValueObjects;

public record Identifier
{
    public const int IDENTIFIER_MIN_LENGTH = 3;
    public const int IDENTIFIER_MAX_LENGTH = 100;
    public string Value { get; }
    
    private Identifier(string value)
    {
        Value = value;
    }

    public static Result<Identifier, AppError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < IDENTIFIER_MIN_LENGTH || value.Length > IDENTIFIER_MAX_LENGTH)
        {
            return AppErrors.ValueIsInvalid("Identifier");
        }

        return new Identifier(value);
    }
}