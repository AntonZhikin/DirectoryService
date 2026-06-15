using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TimeZone = DirectoryService.Domain.Locations.ValueObjects.TimeZone;

namespace DirectoryService.Application.Locations.Create;

public class CreateLocationHandler(
    IValidator<CreateLocationCommand> validator, 
    ILocationRepository locationRepository,
    ILogger<CreateLocationHandler> logger)
{
    public async Task<Result<LocationId, AppError>> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToAppError();

        bool isNameTaken = await locationRepository.IsNameTakenAsync(command.Request.Name, cancellationToken);
        if (isNameTaken)
            return AppErrors.AlreadyExists("location name");

        var addressResult = Address.Create(
            command.Request.Address.City, 
            command.Request.Address.Street,
            command.Request.Address.HouseNumber,
            command.Request.Address.Number).Value;
        var nameResult = LocationName.Create(command.Request.Name).Value;
        var timeResult = TimeZone.Create(command.Request.TimeZone).Value;
        
        var location = new Location(nameResult, timeResult, addressResult);
        
        var result = await locationRepository.AddAsync(location, cancellationToken);
        if (result.IsFailure)
            return result.Error;
        
        logger.LogInformation("Location created successfully with Id {LocationId}", location.Id);
        
        return location.Id;
    }
}