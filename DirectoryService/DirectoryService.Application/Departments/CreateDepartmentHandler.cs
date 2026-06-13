using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.DepartmentLocation;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Location;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Departments;

public class CreateDepartmentHandler(
    IValidator<CreateDepartmentCommand> validator,
    IDepartmentRepository departmentRepository,
    ILogger<CreateDepartmentHandler> logger)
{
    public async Task<Result<DepartmentId, AppErrorList>> Handle(
        CreateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToList();

        var request = command.Request;

        if (request.LocationIds.Count > 0)
        {
            bool locationsExist = await departmentRepository.AreLocationsExistAsync(request.LocationIds, cancellationToken);
            if (!locationsExist)
                return AppErrors.NotFound("one or more locationIds").ToErrorList();
        }

        var name = DepartmentName.Create(request.Name).Value;
        var identifier = Identifier.Create(request.Slug).Value;

        var newDepartmentId = new DepartmentId(Guid.NewGuid());

        var departmentLocations = request.LocationIds
            .Select(id => new DepartmentLocation
            {
                Id = new DepartmentLocationId(Guid.NewGuid()),
                LocationId = new LocationId(id),
                DepartmentId = newDepartmentId,
                IsPrimary = false,
            })
            .ToList();

        Result<Department, AppError> departmentResult;

        if (request.ParentId is not null)
        {
            var parent = await departmentRepository.FindByIdAsync(
                new DepartmentId(request.ParentId.Value), cancellationToken);

            if (parent is null)
                return AppErrors.NotFound(request.ParentId.Value).ToErrorList();

            departmentResult = Department.CreateChild(name, identifier, parent, departmentLocations, newDepartmentId);
        }
        else
        {
            departmentResult = Department.CreateParent(name, identifier, departmentLocations, newDepartmentId);
        }

        if (departmentResult.IsFailure)
            return AppErrors.Failure("create_department", "Failed to create department").ToErrorList();

        var result = await departmentRepository.AddAsync(departmentResult.Value, cancellationToken);
        if (result.IsFailure)
            return result.Error;

        logger.LogInformation(
            "Department created: Id={DepartmentId}, Slug={Slug}, Path={Path}",
            departmentResult.Value.Id.Value,
            departmentResult.Value.Slug,
            departmentResult.Value.Path.Value);

        return result.Value;
    }
}
