using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database.Repository;
using DirectoryService.Domain.Positions;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Positions.Delete;

public class DeletePositionHandler(
    IPositionRepository positionRepository,
    ILogger<DeletePositionHandler> logger)
    : ICommandHandler<PositionId, DeletePositionCommand>
{
    public async Task<Result<PositionId, AppError>> Handle(
        DeletePositionCommand command, CancellationToken cancellationToken)
    {
        var positionId = new PositionId(command.PositionId);

        var position = await positionRepository.FindByIdAsync(positionId, cancellationToken);
        if (position is null)
        {
            logger.LogWarning("Position {PositionId} not found", command.PositionId);
            return AppErrors.NotFound(name: "position");
        }

        bool hasLinks = await positionRepository.HasDepartmentLinksAsync(positionId, cancellationToken);
        if (hasLinks)
            return AppErrors.AlreadyExists("position has linked departments");

        positionRepository.Remove(position);

        logger.LogInformation("Position {PositionId} deleted", position.Id.Value);

        return position.Id;
    }
}
