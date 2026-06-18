using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.Departments;

namespace DirectoryService.Application.Departments.UnlinkingPosition;

public record UnlinkingPositionCommand(Guid DepartmentId, Guid PositionId) : ICommand<DepartmentId>;
