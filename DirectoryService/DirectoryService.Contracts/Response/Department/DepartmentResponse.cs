namespace DirectoryService.Contracts.Response.Department;

public record DepartmentResponse(
    Guid Id,
    Guid? ParentId,
    string Name,
    string Identifier,
    string Path,
    int Depth,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
