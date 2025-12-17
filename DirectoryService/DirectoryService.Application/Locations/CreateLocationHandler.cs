using CSharpFunctionalExtensions;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Request;
using DirectoryService.Domain.Location;
using DirectoryService.Domain.Location.ValueObjects;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;
using TimeZone = DirectoryService.Domain.Location.ValueObjects.TimeZone;

namespace DirectoryService.Application.Locations;

public record CreateLocationCommand(CreateLocationRequest Request);

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(x => x.Request)
            .NotNull()
            .WithError(AppErrors.ValueIsInvalid("request"));

        When(x => true, () =>
        {
            RuleFor(x => x.Request.Name).MustBeValueObject(LocationName.Create);
            
            RuleFor(x => x.Request.TimeZone).MustBeValueObject(TimeZone.Create);

            RuleFor(x => x.Request.City)
                .NotNull()
                .WithError(AppErrors.ValueIsInvalid("city"));
            
            RuleFor(x => x.Request.Street)
                .NotNull()
                .WithError(AppErrors.ValueIsInvalid("street"));
            
            RuleFor(x => x.Request.City)
                .NotNull()
                .WithError(AppErrors.ValueIsInvalid("city"));

            RuleFor(x => x.Request.Number)
                .NotNull()
                .WithError(AppErrors.ValueIsInvalid("number"));
            
            RuleFor(x => x.Request.HouseNumber)
                .NotNull()
                .WithError(AppErrors.ValueIsInvalid("house_number"));
        });
    }
}

public class CreateLocationHandler(IValidator<CreateLocationCommand> validator, ILocationRepository locationRepository)
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
        
        return location.Id;
    }
}