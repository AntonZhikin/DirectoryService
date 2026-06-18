using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database.Repository;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Positions.Create;

public class CreatePositionHandler(
    IPositionRepository positionRepository,
    ILogger<CreatePositionHandler> logger)
    : ICommandHandler<PositionId, CreatePositionCommand>
{
    public async Task<Result<PositionId, AppError>> Handle(
        CreatePositionCommand command, CancellationToken cancellationToken)
    {
        var nameResult = PositionName.Create(command.Request.Name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var descriptionResult = PositionDescription.Create(command.Request.Description);
        if (descriptionResult.IsFailure)
            return descriptionResult.Error;

        var position = new Position(nameResult.Value, descriptionResult.Value);

        positionRepository.Add(position);

        logger.LogInformation("Position created with Id {PositionId}", position.Id.Value);

        return position.Id;
    }
}
