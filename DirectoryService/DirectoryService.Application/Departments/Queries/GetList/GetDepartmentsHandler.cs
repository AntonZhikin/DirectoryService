using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Response;
using DirectoryService.Contracts.Response.Department;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Application.Departments.Queries.GetList;

public class GetDepartmentsHandler(IReadDbContext readDbContext)
    : IQueryHandler<PagedResult<DepartmentListItemDto>, GetDepartmentsQuery> 
{
    public async Task<Result<PagedResult<DepartmentListItemDto>, AppError>> Handle(
        GetDepartmentsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var dbQuery = readDbContext.DepartmentsRead;

        if (!string.IsNullOrWhiteSpace(request.Search)) 
            dbQuery = dbQuery.Where(d => EF.Functions.Like(d.Name.Value, $"%{request.Search}%"));
        
        bool sortDesc = string.Equals(request.SortDir, "desc", StringComparison.OrdinalIgnoreCase);

        dbQuery = request.SortBy?.ToLower() switch
        {
            "createdat" => sortDesc
                ? dbQuery.OrderByDescending(d => d.CreatedAt)
                : dbQuery.OrderBy(d => d.CreatedAt),
            _ => sortDesc
                ? dbQuery.OrderByDescending(d => d.Name.Value)
                : dbQuery.OrderBy(d => d.Name.Value),
        };

        var items = await dbQuery
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new DepartmentListItemDto(
                d.Id.Value,
                d.Name.Value,
                d.Path.Value,
                d.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<DepartmentListItemDto>
        {
            Items = items,
            TotalCount = await dbQuery.CountAsync(cancellationToken),
            Page = request.Page,
            PageSize = request.PageSize,
        };
    }
}
