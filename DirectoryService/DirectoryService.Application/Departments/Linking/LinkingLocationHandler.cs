using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
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
    : ICommandHandler<DepartmentLocationId, LinkingLocationCommand>
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
            logger.LogWarning("Department {DepartmentId} not found", command.DepartmentId);
            return AppErrors.NotFound(name: "department");
        }

        var location = await locationRepository.FindByIdAsync(locationId, cancellationToken);
        if (location == null)
        {
            logger.LogWarning("Location {LocationId} not found", command.LocationId);
            return AppErrors.NotFound(name: "location");
        }

        var addResult = department.AddLocation(locationId);
        if (addResult.IsFailure)
        {
            logger.LogWarning(
                "Location {LocationId} is already linked to department {DepartmentId}",
                command.LocationId, command.DepartmentId);
            return addResult.Error;
        }

        var updateResult = await departmentRepository.SaveChangesAsync(department, cancellationToken);
        if (updateResult.IsFailure)
            return updateResult.Error;

        logger.LogInformation(
            "Location {LocationId} linked to department {DepartmentId} as {DepartmentLocationId}",
            command.LocationId, command.DepartmentId, addResult.Value.Id.Value);

        return addResult.Value.Id;
    }
}
