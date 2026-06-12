using CSharpFunctionalExtensions;
using DirectoryService.Domain.Location;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Application.Database;

public interface ILocationRepository
{
    Task<Result<LocationId, AppErrorList>> AddAsync(Location location, CancellationToken cancellationToken);
    Task<bool> IsNameTakenAsync(string name, CancellationToken cancellationToken);
}