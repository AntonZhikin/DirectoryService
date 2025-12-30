using CSharpFunctionalExtensions;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Location;
using DirectoryService.Domain.Location.ValueObjects;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TimeZone = DirectoryService.Domain.Location.ValueObjects.TimeZone;

namespace DirectoryService.Application.Locations;

public class CreateLocationHandler(
    IValidator<CreateLocationCommand> validator, 
    ILocationRepository locationRepository,
    ILogger<CreateLocationCommandValidator> logger)
{
    public async Task<Result<LocationId, AppErrorList>> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();
        
        var addressResult = Address.Create(command.Request.City, command.Request.Street, command.Request.HouseNumber, command.Request.Number).Value;
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