using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Departments.Unlinking;

public class UnlinkingLocationHandler(
    IValidator<UnlinkingLocationCommand> validator,
    IDepartmentRepository departmentRepository,
    ILogger<UnlinkingLocationHandler> logger)
    : ICommandHandler<DepartmentId, UnlinkingLocationCommand>
{
    public async Task<Result<DepartmentId, AppError>> Handle(
        UnlinkingLocationCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToAppError();

        var department = await departmentRepository.FindByIdAsync(new DepartmentId(command.DepartmentId), cancellationToken);
        if (department == null)
        {
            logger.LogWarning("Department {DepartmentId} not found", command.DepartmentId);
            return AppErrors.NotFound(name: "department");
        }

        var unlinkedDepartment = department.DeleteLocation(new LocationId(command.LocationId));
        if (unlinkedDepartment.IsFailure)
        {
            logger.LogWarning(
                "Location {LocationId} is not linked to department {DepartmentId}",
                command.LocationId, command.DepartmentId);
            return unlinkedDepartment.Error;
        }

        var result = await departmentRepository.SaveChangesAsync(department, cancellationToken);
        if (result.IsFailure)
            return result.Error;

        logger.LogInformation(
            "Location {LocationId} unlinked from department {DepartmentId}",
            command.LocationId, command.DepartmentId);

        return department.Id;
    }
}
