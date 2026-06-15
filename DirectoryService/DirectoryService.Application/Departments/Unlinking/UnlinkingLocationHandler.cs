using CSharpFunctionalExtensions;
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
{
    public async Task<Result<DepartmentId, AppErrorList>> Handle(
        UnlinkingLocationCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToList();
        
        var department = await departmentRepository.FindByIdAsync(new DepartmentId(command.DepartmentId), cancellationToken);
        if (department == null)
        {
            logger.LogDebug("Department with id {CommandDepartmentId} not found", command.DepartmentId);
            return AppErrors.NotFound("Department").ToErrorList();
        }

        var unlinkedDepartment = department.DeleteLocation(new LocationId(command.LocationId));
        if (unlinkedDepartment.IsFailure)
            return unlinkedDepartment.Error.ToErrorList();
        
        var result = await departmentRepository.SaveChangesAsync(department, cancellationToken);
        if (result.IsFailure)
            return result.Error;
        
        return department.Id;
    }
}
