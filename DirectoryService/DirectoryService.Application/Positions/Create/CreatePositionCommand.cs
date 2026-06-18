using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Request.Position;
using DirectoryService.Domain.Positions;

namespace DirectoryService.Application.Positions.Create;

public record CreatePositionCommand(CreatePositionRequest Request) : ICommand<PositionId>;
