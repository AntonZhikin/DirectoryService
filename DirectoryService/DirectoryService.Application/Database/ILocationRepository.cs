using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Application.Database;

public interface ILocationRepository
{
    Task<Result<LocationId, AppError>> AddAsync(Location location, CancellationToken cancellationToken);
    Task<bool> IsNameTakenAsync(string name, CancellationToken cancellationToken);
    Task<Location?> FindByIdAsync(LocationId requestLocationId, CancellationToken cancellationToken);
    Task<Result<LocationId, AppError>> SaveChangesAsync(Location location, CancellationToken cancellationToken);
}