using DirectoryService.Application.Validation;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;

namespace DirectoryService.Application.Departments.Unlinking;

public class UnlinkingLocationCommandValidator : AbstractValidator<UnlinkingLocationCommand>
{
    public UnlinkingLocationCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithError(AppErrors.ValueIsInvalid("departmentId"));

        RuleFor(x => x.LocationId)
            .NotEmpty()
            .WithError(AppErrors.ValueIsInvalid("locationId"));
    }
}
