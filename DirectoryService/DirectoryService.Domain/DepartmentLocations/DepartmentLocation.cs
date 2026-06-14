using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Domain.DepartmentLocations;

public sealed class DepartmentLocation
{
    public DepartmentLocationId Id { get; init; }
    public bool IsPrimary { get; init; }
    public required LocationId LocationId { get; init; }
    public required DepartmentId DepartmentId { get; init; }
}