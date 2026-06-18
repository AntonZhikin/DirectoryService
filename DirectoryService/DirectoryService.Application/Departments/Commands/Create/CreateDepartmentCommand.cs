using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Request.Department;
using DirectoryService.Domain.Departments;

namespace DirectoryService.Application.Departments.Commands.Create;

public record CreateDepartmentCommand(CreateDepartmentRequest Request) : ICommand<DepartmentId>;
