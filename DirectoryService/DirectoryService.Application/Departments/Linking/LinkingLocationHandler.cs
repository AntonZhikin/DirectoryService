using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Locations;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Departments.Linking;

public class LinkingLocationHandler(
    IValidator<LinkingLocationCommand> validator,
    IDepartmentRepository departmentRepository,
    ILocationRepository locationRepository,
    ILogger<LinkingLocationHandler> logger)
{
    public async Task<Result<DepartmentLocationId, AppError>> Handle(
        LinkingLocationCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToAppError();

        var departmentId = new DepartmentId(command.DepartmentId);
        var locationId = new LocationId(command.LocationId);

        var department = await departmentRepository.FindByIdAsync(departmentId, cancellationToken);
        if (department == null)
        {
            logger.LogDebug("Department not found");
            return AppErrors.NotFound(name: "department");
        }

        var location = await locationRepository.FindByIdAsync(locationId, cancellationToken);
        if (location == null)
        {
            logger.LogDebug("Location not found");
            return AppErrors.NotFound(name: "location");
        }

        var addResult = department.AddLocation(locationId);
        if (addResult.IsFailure)
        {
            logger.LogDebug("Relationship already exists");
            return addResult.Error;
        }

        var updateResult = await departmentRepository.SaveChangesAsync(department, cancellationToken);
        if (updateResult.IsFailure)
            return updateResult.Error;

        return addResult.Value.Id;
    }
}
