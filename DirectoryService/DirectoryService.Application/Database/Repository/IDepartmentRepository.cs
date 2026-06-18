using DirectoryService.Domain.Departments;

namespace DirectoryService.Application.Database.Repository;

public interface IDepartmentRepository
{
    void Add(Department department);
    Task<Department?> FindByIdAsync(DepartmentId id, CancellationToken cancellationToken);
    Task<Department?> FindByIdWithPositionsAsync(DepartmentId id, CancellationToken cancellationToken);
    Task<bool> AreLocationsExistAsync(IEnumerable<Guid> locationIds, CancellationToken cancellationToken);
    void Remove(Department department);
    Task<bool> HasChildrenAsync(DepartmentId id, CancellationToken cancellationToken);
}
