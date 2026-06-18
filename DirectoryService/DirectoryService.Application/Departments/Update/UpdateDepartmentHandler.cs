using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
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
    ITransactionManager transactionManager,
    ILogger<UpdateDepartmentHandler> logger)
    : ICommandHandler<DepartmentId, UpdateDepartmentCommand>
{
    public async Task<Result<DepartmentId, AppError>> Handle(
        UpdateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var validateResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validateResult.IsValid)
            return validateResult.ToAppError();

        var department = await departmentRepository
            .FindByIdAsync(new DepartmentId(command.DepartmentId), cancellationToken);
        if (department == null)
        {
            logger.LogWarning("Department {DepartmentId} not found", command.DepartmentId);
            return AppErrors.NotFound(name: "department");
        }

        var updateResult = department.UpdateName(command.Request.Name);
        if (updateResult.IsFailure)
            return updateResult.Error;

        var saveResult = await transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        logger.LogInformation("Department {DepartmentId} renamed to {Name}", department.Id.Value, command.Request.Name);

        return department.Id;
    }
}
