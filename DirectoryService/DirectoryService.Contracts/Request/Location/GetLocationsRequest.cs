namespace DirectoryService.Contracts.Request.Location;

public record GetLocationsRequest(
    string? Search,
    string? SortBy,
    string? SortDir,
    int? MinDepartmentCount,
    int PageSize = 20,
    int Page = 1);
