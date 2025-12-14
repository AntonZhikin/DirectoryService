using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Location;

namespace DirectoryService.Domain.DepartamentLocation;

public sealed class DepartmentLocation
{
    public DepartmentLocationId Id { get; init; }
    public required LocationId LocationId { get; init; }
    public required DepartmentId DepartmentId { get; init; }
}