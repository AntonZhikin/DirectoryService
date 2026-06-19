using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.Departments;

namespace DirectoryService.Application.Departments.Commands.Delete;

public record DeleteDepartmentCommand(Guid DepartmentId) : ICommand<DepartmentId>;
