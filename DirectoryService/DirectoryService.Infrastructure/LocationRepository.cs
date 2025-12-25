using CSharpFunctionalExtensions;
using DirectoryService.Application;
using DirectoryService.Domain.Location;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Infrastructure;

public class LocationRepository(ApplicationDbContext dbContext) : ILocationRepository
{
    public async Task<Result<LocationId, AppErrorList>> AddAsync(Location location, CancellationToken cancelationToken = default)
    {
        dbContext.Locations.Add(location);

        await dbContext.SaveChangesAsync(cancelationToken);
        
        return location.Id;
    }
}