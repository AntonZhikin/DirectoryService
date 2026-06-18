using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database.Repository;
using DirectoryService.Domain.Locations;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Locations.Delete;

public class DeleteLocationHandler(
    ILocationRepository locationRepository,
    ILogger<DeleteLocationHandler> logger)
    : ICommandHandler<LocationId, DeleteLocationCommand>
{
    public async Task<Result<LocationId, AppError>> Handle(
        DeleteLocationCommand command, CancellationToken cancellationToken)
    {
        var locationId = new LocationId(command.LocationId);

        var location = await locationRepository.FindByIdAsync(locationId, cancellationToken);
        if (location is null)
        {
            logger.LogWarning("Location {LocationId} not found", command.LocationId);
            return AppErrors.NotFound(name: "location");
        }

        bool hasLinks = await locationRepository.HasDepartmentLinksAsync(locationId, cancellationToken);
        if (hasLinks)
            return AppErrors.AlreadyExists("location has linked departments");

        locationRepository.Remove(location);

        logger.LogInformation("Location {LocationId} deleted", location.Id.Value);

        return location.Id;
    }
}
