using DirectoryService.Application.Validation;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;

namespace DirectoryService.Application.Departments.Commands.Linking;

public class LinkingLocationCommandValidator : AbstractValidator<LinkingLocationCommand>
{
    public LinkingLocationCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithError(AppErrors.ValueIsInvalid("departmentId"));

        RuleFor(x => x.LocationId)
            .NotEmpty()
            .WithError(AppErrors.ValueIsInvalid("locationId"));
    }
}
