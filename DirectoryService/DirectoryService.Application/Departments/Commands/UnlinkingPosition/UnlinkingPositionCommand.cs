using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.Departments;

namespace DirectoryService.Application.Departments.Commands.UnlinkingPosition;

public record UnlinkingPositionCommand(Guid DepartmentId, Guid PositionId) : ICommand<DepartmentId>;
