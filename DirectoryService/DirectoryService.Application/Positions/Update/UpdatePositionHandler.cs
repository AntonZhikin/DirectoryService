using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database.Repository;
using DirectoryService.Domain.Positions;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Positions.Update;

public class UpdatePositionHandler(
    IPositionRepository positionRepository,
    ILogger<UpdatePositionHandler> logger)
    : ICommandHandler<PositionId, UpdatePositionCommand>
{
    public async Task<Result<PositionId, AppError>> Handle(
        UpdatePositionCommand command, CancellationToken cancellationToken)
    {
        var position = await positionRepository.FindByIdAsync(
            new PositionId(command.PositionId), cancellationToken);
        if (position is null)
        {
            logger.LogWarning("Position {PositionId} not found", command.PositionId);
            return AppErrors.NotFound(name: "position");
        }

        var updateResult = position.Update(command.Request.Name, command.Request.Description);
        if (updateResult.IsFailure)
            return updateResult.Error;

        logger.LogInformation("Position {PositionId} updated", position.Id.Value);

        return position.Id;
    }
}
