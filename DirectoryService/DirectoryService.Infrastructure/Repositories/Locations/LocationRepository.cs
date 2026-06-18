using DirectoryService.Application.Database;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Repositories.Locations;

public class LocationRepository(ApplicationDbContext dbContext) : ILocationRepository
{
    public void Add(Location location)
    {
        dbContext.Locations.Add(location);
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
}
