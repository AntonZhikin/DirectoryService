using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Request.Department;
using DirectoryService.Domain.Departments;

namespace DirectoryService.Application.Departments.Update;

public record UpdateDepartmentCommand(Guid DepartmentId, UpdateDepartmentRequest Request) : ICommand<DepartmentId>;
