using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Application.Database;

public interface IDepartmentRepository
{
    Task<Result<DepartmentId, AppErrorList>> AddAsync(Department department, CancellationToken cancellationToken);
    Task<Department?> FindByIdAsync(DepartmentId id, CancellationToken cancellationToken);
    Task<bool> AreLocationsExistAsync(IEnumerable<Guid> locationIds, CancellationToken cancellationToken);
}
