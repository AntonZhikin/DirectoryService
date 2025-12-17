using CSharpFunctionalExtensions;
using DirectoryService.Domain.Location;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Application;

public interface ILocationRepository
{
    Task<Result<LocationId, AppErrorList>> AddAsync(Location location, CancellationToken cancellationToken);
}