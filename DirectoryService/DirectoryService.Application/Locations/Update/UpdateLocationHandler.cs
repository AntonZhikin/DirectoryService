using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Locations;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Locations.Update;

public class UpdateLocationHandler(
    IValidator<UpdateLocationCommand> validator,
    ILocationRepository locationRepository,
    ILogger<UpdateLocationHandler> logger
    )
    : ICommandHandler<LocationId, UpdateLocationCommand>
{
    public async Task<Result<LocationId, AppError>> Handle(
        UpdateLocationCommand command,
        CancellationToken cancellationToken)
    {
        var validateResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validateResult.IsValid)
            return validateResult.ToAppError();

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

        await locationRepository.SaveChangesAsync(location, cancellationToken);

        logger.LogInformation("Location {LocationId} updated", location.Id.Value);

        return location.Id;
    }
}