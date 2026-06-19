using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Response.Department;
using DirectoryService.Domain.Departments;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Application.Departments.Queries.GetById;

public class GetDepartmentByIdHandler(IReadDbContext readDbContext)
    : IQueryHandler<DepartmentDto, GetDepartmentByIdQuery>
{
    public async Task<Result<DepartmentDto, AppError>> Handle(
        GetDepartmentByIdQuery query,
        CancellationToken cancellationToken)
    {
        var dto = await readDbContext.DepartmentsRead
            .Where(d => d.Id == new DepartmentId(query.DepartmentId))
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
            return AppErrors.NotFound(query.DepartmentId, "department");

        return new DepartmentDto(
            dto.Id.Value,
            dto.ParentId?.Value,
            dto.Name.Value,
            dto.Identifier.Value,
            dto.Path.Value,
            dto.Depth);
    }
}
