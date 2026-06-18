using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Locations;
using DirectoryService.Shared.ErrorManagement;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Departments.Create;

public class CreateDepartmentHandler(
    IValidator<CreateDepartmentCommand> validator,
    IDepartmentRepository departmentRepository,
    ITransactionManager transactionManager,
    ILogger<CreateDepartmentHandler> logger) : ICommandHandler<DepartmentId, CreateDepartmentCommand>
{
    public async Task<Result<DepartmentId, AppError>> Handle(
        CreateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToAppError();

        var request = command.Request;

        var transactionResult = await transactionManager.BeginTransaction(cancellationToken);
        if (transactionResult.IsFailure)
            return transactionResult.Error;

        using var transaction = transactionResult.Value;

        if (request.LocationIds.Count > 0)
        {
            bool locationsExist = await departmentRepository.AreLocationsExistAsync(request.LocationIds, cancellationToken);
            if (!locationsExist)
                return AppErrors.NotFound(name: "one or more locationIds");
        }

        var nameResult = DepartmentName.Create(request.Name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var identifierResult = Identifier.Create(request.Slug);
        if (identifierResult.IsFailure)
            return identifierResult.Error;

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
                return AppErrors.NotFound(request.ParentId.Value);

            departmentResult = Department.CreateChild(nameResult.Value, identifierResult.Value, parent, departmentLocations, newDepartmentId);
        }
        else
        {
            departmentResult = Department.CreateParent(nameResult.Value, identifierResult.Value, departmentLocations, newDepartmentId);
        }

        if (departmentResult.IsFailure)
            return AppErrors.Failure("Failed to create department");

        departmentRepository.Add(departmentResult.Value);

        var saveResult = await transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            transaction.Rollback();
            return saveResult.Error;
        }

        var commitResult = transaction.Commit();
        if (commitResult.IsFailure)
            return commitResult.Error;

        logger.LogInformation(
            "Department created: Id={DepartmentId}, Slug={Slug}, Path={Path}",
            departmentResult.Value.Id.Value,
            departmentResult.Value.Slug,
            departmentResult.Value.Path.Value);

        return departmentResult.Value.Id;
    }
}
