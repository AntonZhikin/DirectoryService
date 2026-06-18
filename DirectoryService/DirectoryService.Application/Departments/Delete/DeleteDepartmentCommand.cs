using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.Departments;

namespace DirectoryService.Application.Departments.Delete;

public record DeleteDepartmentCommand(Guid DepartmentId) : ICommand<DepartmentId>;
