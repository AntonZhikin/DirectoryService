using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;

namespace DirectoryService.Application.Departments.Commands.Update;

public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator()
    {
        RuleFor(x => x.Request)
            .NotNull()
            .WithError(AppErrors.ValueIsInvalid("request"));

        When(_ => true, () =>
        {
            RuleFor(x => x.Request.Name).MustBeValueObject(DepartmentName.Create);
        });
    }
}
