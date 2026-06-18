using DirectoryService.Application.Validation;
using DirectoryService.Domain.Positions.ValueObjects;
using FluentValidation;

namespace DirectoryService.Application.Positions.Update;

public class UpdatePositionCommandValidator : AbstractValidator<UpdatePositionCommand>
{
    public UpdatePositionCommandValidator()
    {
        RuleFor(x => x.Request.Name).MustBeValueObject(PositionName.Create);
        RuleFor(x => x.Request.Description).MustBeValueObject(PositionDescription.Create);
    }
}
