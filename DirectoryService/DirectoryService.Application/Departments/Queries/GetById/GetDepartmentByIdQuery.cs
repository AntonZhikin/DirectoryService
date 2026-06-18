using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Response.Department;

namespace DirectoryService.Application.Departments.Queries.GetById;

public record GetDepartmentByIdQuery(Guid DepartmentId) : IQuery<DepartmentDto>;
