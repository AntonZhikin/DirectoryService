using DirectoryService.Application.Validation;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;

namespace DirectoryService.Application.Departments.Commands.UnlinkingPosition;

public class UnlinkingPositionCommandValidator : AbstractValidator<UnlinkingPositionCommand>
{
    public UnlinkingPositionCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithError(AppErrors.ValueIsInvalid("departmentId"));

        RuleFor(x => x.PositionId)
            .NotEmpty()
            .WithError(AppErrors.ValueIsInvalid("positionId"));
    }
}
