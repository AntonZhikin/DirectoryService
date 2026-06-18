using DirectoryService.Application.Database;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Repositories.Departments;

public class DepartmentRepository(ApplicationDbContext dbContext) : IDepartmentRepository
{
    public void Add(Department department) => dbContext.Departments.Add(department);

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
}
