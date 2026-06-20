namespace DirectoryService.Contracts.Response.Department;

public record DepartmentListItemDto(
    Guid Id,
    string Name,
    string Path,
    DateTime CreatedAt);
