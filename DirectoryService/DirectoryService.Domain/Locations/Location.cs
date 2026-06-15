using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Shared.ErrorManagement;
using TimeZone = DirectoryService.Domain.Locations.ValueObjects.TimeZone;

namespace DirectoryService.Domain.Locations;

public sealed class Location
{
    // EF Core
    private Location()
    {
    }
    
    private List<DepartmentLocations.DepartmentLocation> _departmentLocations = [];
    public IReadOnlyList<DepartmentLocations.DepartmentLocation> DepartmentLocations => _departmentLocations;

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

    public UnitResult<AppError> Update(string name, string city, string street, string houseNumber, string number, bool active, string timeZone)
    {
        var nameResult = LocationName.Create(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var addressResult = Address.Create(city, street, houseNumber, number);
        if (addressResult.IsFailure)
            return addressResult.Error;
        
        var timeZoneResult = TimeZone.Create(timeZone);
        if (timeZoneResult.IsFailure)
            return  timeZoneResult.Error;
        
        Name = nameResult.Value;
        Address = addressResult.Value;
        IsActive = active;
        TimeZone = timeZoneResult.Value;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<AppError>();
    }
}