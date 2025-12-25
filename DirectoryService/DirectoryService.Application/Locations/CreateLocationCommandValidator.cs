using DirectoryService.Application.Validation;
using DirectoryService.Domain.Location.ValueObjects;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;
using TimeZone = DirectoryService.Domain.Location.ValueObjects.TimeZone;

namespace DirectoryService.Application.Locations;

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{ public CreateLocationCommandValidator()
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