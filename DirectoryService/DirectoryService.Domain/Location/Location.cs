using System.Runtime.InteropServices.JavaScript;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartamentLocation;

namespace DirectoryService.Domain.Location;

public sealed class Location
{
    // EF Core
    private Location()
    {
    }

    private List<Address> _addresses = [];
    private List<DepartmentLocation> _departmentLocations = [];
    public IReadOnlyList<Address> Addresses => _addresses;
    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations;

    public LocationId Id { get; private set; } = null!;

    public LocationName Name { get; private set; } = null!;

    public Timezone Timezone { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public Location(LocationName name, Timezone timezone, Address address)
    {
        Id = new LocationId(Guid.NewGuid());
        Name = name;
        Timezone = timezone;
        IsActive = true;
        _addresses.Add(address);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}