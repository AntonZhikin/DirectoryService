using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.Departments;

namespace DirectoryService.Application.Departments.Commands.Unlinking;

public record UnlinkingLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand<DepartmentId>;
