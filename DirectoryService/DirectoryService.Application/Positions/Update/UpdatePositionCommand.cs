using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Request.Position;
using DirectoryService.Domain.Positions;

namespace DirectoryService.Application.Positions.Update;

public record UpdatePositionCommand(Guid PositionId, UpdatePositionRequest Request) : ICommand<PositionId>;
