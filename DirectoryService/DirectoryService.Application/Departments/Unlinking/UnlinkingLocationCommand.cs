using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Departments.Unlinking;

public record UnlinkingLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand;
