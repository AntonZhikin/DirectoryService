using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.Departments;

namespace DirectoryService.Application.Departments.Unlinking;

public record UnlinkingLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand<DepartmentId>;
