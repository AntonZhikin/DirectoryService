using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database.Repository;
using DirectoryService.Domain.Locations;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Locations.Commands.Update;

public class UpdateLocationHandler(
    ILocationRepository locationRepository,
    ILogger<UpdateLocationHandler> logger)
    : ICommandHandler<LocationId, UpdateLocationCommand>
{
    public async Task<Result<LocationId, AppError>> Handle(
        UpdateLocationCommand command,
        CancellationToken cancellationToken)
    {
        var location = await locationRepository.FindByIdAsync(new LocationId(command.LocationId), cancellationToken);
        if (location == null)
        {
            logger.LogWarning("Location {LocationId} not found", command.LocationId);
            return AppErrors.NotFound(name: "location");
        }

        var request = command.Request;

        var updateResult = location.Update(request.Name, request.Address.City, request.Address.Street,
            request.Address.HouseNumber, request.Address.Number, request.IsActive, request.TimeZone);
        if (updateResult.IsFailure)
            return updateResult.Error;

        logger.LogInformation("Location {LocationId} updated", location.Id.Value);

        return location.Id;
    }
}
