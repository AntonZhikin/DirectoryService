using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.Positions;

namespace DirectoryService.Application.Positions.Delete;

public record DeletePositionCommand(Guid PositionId) : ICommand<PositionId>;
