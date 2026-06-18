using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database.Repository;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.Extensions.Logging;
using TimeZone = DirectoryService.Domain.Locations.ValueObjects.TimeZone;

namespace DirectoryService.Application.Locations.Commands.Create;

public class CreateLocationHandler(
    ILocationRepository locationRepository,
    ILogger<CreateLocationHandler> logger)
    : ICommandHandler<LocationId, CreateLocationCommand>
{
    public async Task<Result<LocationId, AppError>> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        bool isNameTaken = await locationRepository.IsNameTakenAsync(command.Request.Name, cancellationToken);
        if (isNameTaken)
            return AppErrors.AlreadyExists("location name");

        var addressResult = Address.Create(
            command.Request.Address.City,
            command.Request.Address.Street,
            command.Request.Address.HouseNumber,
            command.Request.Address.Number);
        if (addressResult.IsFailure)
            return addressResult.Error;

        var nameResult = LocationName.Create(command.Request.Name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var timeResult = TimeZone.Create(command.Request.TimeZone);
        if (timeResult.IsFailure)
            return timeResult.Error;

        var location = new Location(nameResult.Value, timeResult.Value, addressResult.Value);

        locationRepository.Add(location);

        logger.LogInformation("Location created successfully with Id {LocationId}", location.Id);

        return location.Id;
    }
}
