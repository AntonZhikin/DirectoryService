using DirectoryService.Domain.Locations;

namespace DirectoryService.Application.Database.Repository;

public interface ILocationRepository
{
    void Add(Location location);
    Task<bool> IsNameTakenAsync(string name, CancellationToken cancellationToken);
    Task<Location?> FindByIdAsync(LocationId requestLocationId, CancellationToken cancellationToken);
    void Remove(Location location);
    Task<bool> HasDepartmentLinksAsync(LocationId id, CancellationToken cancellationToken);
}