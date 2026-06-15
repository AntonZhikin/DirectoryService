using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Locations;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Repositories.Locations;

public class EfCoreLocationRepository(ApplicationDbContext dbContext) : ILocationRepository
{
    public async Task<Result<LocationId, AppError>> AddAsync(Location location, CancellationToken cancellationToken = default)
    {
        dbContext.Locations.Add(location);
        await dbContext.SaveChangesAsync(cancellationToken);
        return location.Id;
    }

    public async Task<bool> IsNameTakenAsync(string name, CancellationToken cancellationToken = default)
    {
        return await dbContext.Locations
            .AnyAsync(l => l.Name.Value == name, cancellationToken);
    }

    public async Task<Location?> FindByIdAsync(LocationId requestLocationId, CancellationToken cancellationToken)
    {
        return await dbContext.Locations.FirstOrDefaultAsync(l => l.Id == requestLocationId, cancellationToken);
    }

    public async Task<Result<LocationId, AppError>> SaveChangesAsync(Location location, CancellationToken cancellationToken)
    {
        // location загружена трекингом в FindByIdAsync, изменения попадут в SaveChanges сами.
        await dbContext.SaveChangesAsync(cancellationToken);

        return location.Id;
    }
}
