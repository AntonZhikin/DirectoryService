using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartamentLocation;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Shared.ErrorManagement;
using Path = DirectoryService.Domain.Departments.ValueObjects.Path;

namespace DirectoryService.Domain.Departments;

public sealed class Department
{
    // EfCore
    private Department() { }
    
    private readonly List<DepartmentLocation> _departmentLocations = [];
    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations;
    public DepartmentId Id { get; private set; }
    public DepartmentId? ParentId { get; private set; }
    public Department? Parent { get; private set; }
    public DepartmentName Name { get; private set; }
    public Identifier Identifier { get; private set; }
    public Path Path { get; private set; }
    public int Depth { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public List<Department> ChildrenDepartments = [];
    private Department(
        DepartmentId id,
        DepartmentId? parentId,
        DepartmentName name,
        Identifier identifier,
        Path path,
        int depth,
        IEnumerable<DepartmentLocation> departmentLocations)
    {
        Id = id;
        ParentId = parentId;
        Name = name;
        Identifier = identifier;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Path = path;
        Depth = depth;
        _departmentLocations = departmentLocations.ToList();
    }

    public static Result<Department, AppError> CreateParent(
        DepartmentName name,
        Identifier identifier,
        IEnumerable<DepartmentLocation> departmentLocations,
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
        IEnumerable<DepartmentLocation> departmentLocations,
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
}