using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database.Repository;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Positions;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Departments.LinkingPosition;

public class LinkingPositionHandler(
    IDepartmentRepository departmentRepository,
    IPositionRepository positionRepository,
    ILogger<LinkingPositionHandler> logger)
    : ICommandHandler<DepartmentPositionId, LinkingPositionCommand>
{
    public async Task<Result<DepartmentPositionId, AppError>> Handle(
        LinkingPositionCommand command, CancellationToken cancellationToken)
    {
        var departmentId = new DepartmentId(command.DepartmentId);
        var positionId = new PositionId(command.PositionId);

        var department = await departmentRepository.FindByIdWithPositionsAsync(departmentId, cancellationToken);
        if (department is null)
        {
            logger.LogWarning("Department {DepartmentId} not found", command.DepartmentId);
            return AppErrors.NotFound(name: "department");
        }

        var position = await positionRepository.FindByIdAsync(positionId, cancellationToken);
        if (position is null)
        {
            logger.LogWarning("Position {PositionId} not found", command.PositionId);
            return AppErrors.NotFound(name: "position");
        }

        var addResult = department.AddPosition(positionId);
        if (addResult.IsFailure)
        {
            logger.LogWarning(
                "Position {PositionId} is already linked to department {DepartmentId}",
                command.PositionId, command.DepartmentId);
            return addResult.Error;
        }

        logger.LogInformation(
            "Position {PositionId} linked to department {DepartmentId} as {DepartmentPositionId}",
            command.PositionId, command.DepartmentId, addResult.Value.Id.Value);

        return addResult.Value.Id;
    }
}
