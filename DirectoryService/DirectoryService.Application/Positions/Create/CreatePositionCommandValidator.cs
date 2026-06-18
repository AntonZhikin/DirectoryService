using DirectoryService.Application.Validation;
using DirectoryService.Domain.Positions.ValueObjects;
using FluentValidation;

namespace DirectoryService.Application.Positions.Create;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        RuleFor(x => x.Request.Name).MustBeValueObject(PositionName.Create);
        RuleFor(x => x.Request.Description).MustBeValueObject(PositionDescription.Create);
    }
}
