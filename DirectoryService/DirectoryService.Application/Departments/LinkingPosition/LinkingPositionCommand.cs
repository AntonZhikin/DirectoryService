using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.DepartmentPositions;

namespace DirectoryService.Application.Departments.LinkingPosition;

public record LinkingPositionCommand(Guid DepartmentId, Guid PositionId) : ICommand<DepartmentPositionId>;
