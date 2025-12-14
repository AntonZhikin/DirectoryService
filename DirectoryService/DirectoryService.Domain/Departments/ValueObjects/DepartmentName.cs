using CSharpFunctionalExtensions;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Domain.Departments.ValueObjects;

public record DepartmentName
{
    public const int NAME_MIN_LENGTH = 3;
    public const int NAME_MAX_LENGTH = 100;
    public string Value { get; }
    
    private DepartmentName(string value)
    {
        Value = value;
    }

    public static Result<DepartmentName, AppError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < NAME_MIN_LENGTH || value.Length > NAME_MAX_LENGTH)
        {
            return AppErrors.ValueIsInvalid("Department Name");
        }

        return new DepartmentName(value);
    }
}