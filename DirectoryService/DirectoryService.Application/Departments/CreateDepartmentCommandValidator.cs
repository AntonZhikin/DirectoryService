using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;

namespace DirectoryService.Application.Departments;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.Request)
            .NotNull()
            .WithError(AppErrors.ValueIsInvalid("request"));

        When(x => x.Request != null, () =>
        {
            RuleFor(x => x.Request.Name).MustBeValueObject(DepartmentName.Create);
            RuleFor(x => x.Request.Slug).MustBeValueObject(Identifier.Create);

            RuleFor(x => x.Request.LocationIds)
                .NotNull()
                .WithError(AppErrors.ValueIsInvalid("locationIds"));
        });
    }
}
