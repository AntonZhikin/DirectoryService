using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Locations;
using DirectoryService.Shared.ErrorManagement;
using Path = DirectoryService.Domain.Departments.ValueObjects.Path;

namespace DirectoryService.Domain.Departments;

public sealed class Department
{
    // EfCore
    private Department() { }
    
    private readonly List<DepartmentLocation> _locations = [];
    private readonly List<DepartmentPosition> _positions = [];
    public DepartmentId Id { get; private set; }
    public DepartmentId? ParentId { get; private set; }
    public Department? Parent { get; private set; }
    public DepartmentName Name { get; private set; }
    public Identifier Identifier { get; private set; }
    public Path Path { get; private set; }
    public string Slug { get; private set; }
    public int Depth { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    public List<Department> ChildrenDepartments = [];
    public IReadOnlyList<DepartmentLocations.DepartmentLocation> Locations => _locations;
    public IReadOnlyList<DepartmentPosition> Positions => _positions;
    private Department(
        DepartmentId id,
        DepartmentId? parentId,
        DepartmentName name,
        Identifier identifier,
        Path path,
        int depth,
        IEnumerable<DepartmentLocations.DepartmentLocation> departmentLocations)
    {
        Id = id;
        ParentId = parentId;
        Name = name;
        Identifier = identifier;
        Slug = identifier.Value;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Path = path;
        Depth = depth;
        _locations = departmentLocations.ToList();
    }

    public static Result<Department, AppError> CreateParent(
        DepartmentName name,
        Identifier identifier,
        IEnumerable<DepartmentLocations.DepartmentLocation> departmentLocations,
        DepartmentId? departmentId = null)
    {
        var departmentLocationsList = departmentLocations.ToList();

        if (departmentLocationsList.Count == 0)
        {
            return AppErrors.ValueIsInvalid("DepartmentLocation");
        }

        var path = Path.CreateParent(identifier);

        return new Department(
            departmentId ?? new DepartmentId(Guid.NewGuid()),
            null,
            name,
            identifier,
            path,
            0,
            departmentLocationsList);
    }

    public static Result<Department, AppError> CreateChild(
        DepartmentName name,
        Identifier identifier,
        Department parent,
        IEnumerable<DepartmentLocations.DepartmentLocation> departmentLocations,
        DepartmentId? departmentId = null)
    {
        var departmentLocationsList = departmentLocations.ToList();

        if (departmentLocationsList.Count == 0)
        {
            return AppErrors.ValueIsInvalid("DepartmentLocation");
        }

        var path = parent.Path.CreateChild(identifier);

        return new Department(
            departmentId ?? new DepartmentId(Guid.NewGuid()),
            parent.Id,
            name,
            identifier,
            path,
            parent.Depth + 1,
            departmentLocationsList);
    }

    public UnitResult<AppError> UpdateName(string requestName)
    {
        var newName = DepartmentName.Create(requestName);
        if (newName.IsFailure)
            return newName.Error;

        Name = newName.Value;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<AppError>();
    }

    public Result<DepartmentLocation, AppError> AddLocation(LocationId locationId)
    {
        if (_locations.Any(l => l.LocationId == locationId))
            return AppErrors.AlreadyExists("department location");

        var departmentLocation = new DepartmentLocation
        {
            Id = new DepartmentLocationId(Guid.NewGuid()),
            LocationId = locationId,
            DepartmentId = Id,
            IsPrimary = false,
        };

        _locations.Add(departmentLocation);
        UpdatedAt = DateTime.UtcNow;

        return departmentLocation;
    }

    public UnitResult<AppError> DeleteLocation(LocationId locationId)
    {
        var departmentLocation = _locations.FirstOrDefault(l => l.LocationId == locationId);
        if (departmentLocation is null)
            return AppErrors.NotFound(name: "department location");
        
        _locations.Remove(departmentLocation);
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<AppError>();
    }
}

