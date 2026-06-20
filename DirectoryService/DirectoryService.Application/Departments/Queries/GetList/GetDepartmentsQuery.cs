using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Request.Department;
using DirectoryService.Contracts.Response;
using DirectoryService.Contracts.Response.Department;

namespace DirectoryService.Application.Departments.Queries.GetList;

public record GetDepartmentsQuery(GetDepartmentsRequest Request)
    : IQuery<PagedResult<DepartmentListItemDto>>;
