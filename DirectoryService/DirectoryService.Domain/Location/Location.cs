using DirectoryService.Domain.DepartamentLocation;
using DirectoryService.Domain.Location.ValueObjects;
using TimeZone = DirectoryService.Domain.Location.ValueObjects.TimeZone;

namespace DirectoryService.Domain.Location;

public sealed class Location
{
    // EF Core
    private Location()
    {
    }
    
    private List<DepartmentLocation> _departmentLocations = [];
    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations;

    public LocationId Id { get; private set; } = null!;

    public LocationName Name { get; private set; } = null!;
    
    public Address Address { get; private set; } = null!;

    public TimeZone TimeZone { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public Location(LocationName name, TimeZone timeZone, Address address)
    {
        Id = new LocationId(Guid.NewGuid());
        Name = name;
        TimeZone = timeZone;
        IsActive = true;
        Address = address;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}