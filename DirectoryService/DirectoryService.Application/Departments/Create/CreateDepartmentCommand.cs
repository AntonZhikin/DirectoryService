using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Request.Department;

namespace DirectoryService.Application.Departments.Create;

public record CreateDepartmentCommand(CreateDepartmentRequest Request) : ICommand;
