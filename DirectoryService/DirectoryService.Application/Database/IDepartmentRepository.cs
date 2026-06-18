using DirectoryService.Domain.Departments;

namespace DirectoryService.Application.Database;

public interface IDepartmentRepository
{
    void Add(Department department);
    Task<Department?> FindByIdAsync(DepartmentId id, CancellationToken cancellationToken);
    Task<bool> AreLocationsExistAsync(IEnumerable<Guid> locationIds, CancellationToken cancellationToken);
}
