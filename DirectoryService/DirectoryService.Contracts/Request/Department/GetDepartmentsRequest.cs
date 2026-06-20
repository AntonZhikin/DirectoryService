namespace DirectoryService.Contracts.Request.Department;

public record GetDepartmentsRequest(
    string? Search,
    string? SortBy,
    string? SortDir,
    int PageSize = 20,
    int Page = 1);
