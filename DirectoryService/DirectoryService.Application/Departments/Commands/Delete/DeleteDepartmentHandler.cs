using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database.Repository;
using DirectoryService.Domain.Departments;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Departments.Commands.Delete;

public class DeleteDepartmentHandler(
    IDepartmentRepository departmentRepository,
    ILogger<DeleteDepartmentHandler> logger)
    : ICommandHandler<DepartmentId, DeleteDepartmentCommand>
{
    public async Task<Result<DepartmentId, AppError>> Handle(
        DeleteDepartmentCommand command, CancellationToken cancellationToken)
    {
        var departmentId = new DepartmentId(command.DepartmentId);

        var department = await departmentRepository.FindByIdAsync(departmentId, cancellationToken);
        if (department is null)
        {
            logger.LogWarning("Department {DepartmentId} not found", command.DepartmentId);
            return AppErrors.NotFound(name: "department");
        }

        bool hasChildren = await departmentRepository.HasChildrenAsync(departmentId, cancellationToken);
        if (hasChildren)
            return AppErrors.AlreadyExists("department has child departments");

        departmentRepository.Remove(department);

        logger.LogInformation("Department {DepartmentId} deleted", department.Id.Value);

        return department.Id;
    }
}
