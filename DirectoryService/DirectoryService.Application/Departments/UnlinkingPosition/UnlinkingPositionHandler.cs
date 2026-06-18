using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database.Repository;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Departments.UnlinkingPosition;

public class UnlinkingPositionHandler(
    IDepartmentRepository departmentRepository,
    ILogger<UnlinkingPositionHandler> logger)
    : ICommandHandler<DepartmentId, UnlinkingPositionCommand>
{
    public async Task<Result<DepartmentId, AppError>> Handle(
        UnlinkingPositionCommand command, CancellationToken cancellationToken)
    {
        var department = await departmentRepository.FindByIdWithPositionsAsync(
            new DepartmentId(command.DepartmentId), cancellationToken);
        if (department is null)
        {
            logger.LogWarning("Department {DepartmentId} not found", command.DepartmentId);
            return AppErrors.NotFound(name: "department");
        }

        var unlinkResult = department.DeletePosition(new PositionId(command.PositionId));
        if (unlinkResult.IsFailure)
        {
            logger.LogWarning(
                "Position {PositionId} is not linked to department {DepartmentId}",
                command.PositionId, command.DepartmentId);
            return unlinkResult.Error;
        }

        logger.LogInformation(
            "Position {PositionId} unlinked from department {DepartmentId}",
            command.PositionId, command.DepartmentId);

        return department.Id;
    }
}
