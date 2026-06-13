namespace DirectoryService.Contracts.Request.Department;

public record CreateDepartmentRequest(
    string Name,
    string Slug,
    Guid? ParentId,
    List<Guid> LocationIds);
