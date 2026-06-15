using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Repositories.Departments;

public class EfCoreDepartmentRepository(ApplicationDbContext dbContext) : IDepartmentRepository
{
    public async Task<Result<DepartmentId, AppError>> AddAsync(
        Department department,
        CancellationToken cancellationToken)
    {
        dbContext.Departments.Add(department);
        await dbContext.SaveChangesAsync(cancellationToken);
        return department.Id;
    }

    public async Task<Department?> FindByIdAsync(DepartmentId id, CancellationToken cancellationToken)
    {
        return await dbContext.Departments
            .Include(d => d.Locations)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<bool> AreLocationsExistAsync(
        IEnumerable<Guid> locationIds,
        CancellationToken cancellationToken)
    {
        var locationIdObjects = locationIds.Select(id => new LocationId(id)).ToList();
        int foundCount = await dbContext.Locations
            .Where(l => locationIdObjects.Contains(l.Id))
            .CountAsync(cancellationToken);

        return foundCount == locationIdObjects.Count;
    }

    public async Task<Result<DepartmentId, AppError>> SaveChangesAsync(Department department, CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);

        return department.Id;
    }
}
