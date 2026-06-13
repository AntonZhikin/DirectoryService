using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Location;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Repositories;

public class EfCoreLocationRepository(ApplicationDbContext dbContext) : ILocationRepository
{
    public async Task<Result<LocationId, AppErrorList>> AddAsync(Location location, CancellationToken cancellationToken = default)
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
}
