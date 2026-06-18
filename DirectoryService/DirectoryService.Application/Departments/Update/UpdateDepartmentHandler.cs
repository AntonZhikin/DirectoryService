using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database.Repository;
using DirectoryService.Domain.Departments;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Departments.Update;

public class UpdateDepartmentHandler(
    IDepartmentRepository departmentRepository,
    ILogger<UpdateDepartmentHandler> logger)
    : ICommandHandler<DepartmentId, UpdateDepartmentCommand>
{
    public async Task<Result<DepartmentId, AppError>> Handle(
        UpdateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
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

        logger.LogInformation("Department {DepartmentId} renamed to {Name}", department.Id.Value, command.Request.Name);

        return department.Id;
    }
}
