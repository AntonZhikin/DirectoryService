namespace DirectoryService.Contracts.Response.Department;

public record DepartmentDto(
    Guid Id,
    Guid? ParentId,
    string Name,
    string Identifier,
    string Path,
    int Depth);
