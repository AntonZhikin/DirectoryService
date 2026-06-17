using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Departments.Update;

public class UpdateDepartmentHandler(
    IValidator<UpdateDepartmentCommand> validator,
    IDepartmentRepository departmentRepository,
    ILogger<UpdateDepartmentHandler> logger)
{
    public async Task<Result<DepartmentId, AppError>> Handle(
        UpdateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var validateResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validateResult.IsValid)
            return validateResult.ToAppError();

        var departmentResult = await departmentRepository
            .FindByIdAsync(new DepartmentId(command.DepartmentId), cancellationToken);
        if (departmentResult == null)
        {
            logger.LogWarning("Department {DepartmentId} not found", command.DepartmentId);
            return AppErrors.NotFound(name: "department");
        }

        var updateResult = departmentResult.UpdateName(command.Request.Name);
        if (updateResult.IsFailure)
            return updateResult.Error;

        var result = await departmentRepository.SaveChangesAsync(departmentResult, cancellationToken);
        if (result.IsFailure)
            return result.Error;

        logger.LogInformation("Department {DepartmentId} renamed to {Name}", result.Value.Value, command.Request.Name);

        return result.Value;
    }
}