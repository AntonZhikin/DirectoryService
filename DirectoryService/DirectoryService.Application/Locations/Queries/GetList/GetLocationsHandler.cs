using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Dtos;
using DirectoryService.Contracts.Response;
using DirectoryService.Contracts.Response.Location;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Application.Locations.Queries.GetList;

public class GetLocationsHandler(IDbConnectionFactory connectionFactory)
    : IQueryHandler<PagedResult<LocationListItemDto>, GetLocationsQuery>
{
    private static readonly Dictionary<string, string> SortColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["name"]            = "name",
        ["createdAt"]       = "created_at",
        ["departmentCount"] = "department_count",
    };

    public async Task<Result<PagedResult<LocationListItemDto>, AppError>> Handle(
        GetLocationsQuery query, CancellationToken cancellationToken)
    {
        var req = query.Request;

        if (req.Page < 1)
            return AppError.Validation("INVALID_PAGE", "page должен быть >= 1", "page");

        if (req.PageSize > 100)
            return AppError.Validation("INVALID_PAGE_SIZE", "pageSize не может быть больше 100", "pageSize");

        if (req.MinDepartmentCount < 0)
            return AppError.Validation("INVALID_MIN_DEPARTMENT_COUNT", "minDepartmentCount не может быть отрицательным", "minDepartmentCount");

        if (req.Search?.Length > 100)
            return AppError.Validation("INVALID_SEARCH", "search не может быть длиннее 100 символов", "search");

        if (!SortColumns.TryGetValue(req.SortBy ?? "name", out var sortColumn))
            return AppError.Validation("INVALID_SORT_BY", $"sortBy должен быть одним из: {string.Join(", ", SortColumns.Keys)}", "sortBy");

        var sortDir = string.Equals(req.SortDir, "desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

        var parameters = new DynamicParameters();
        parameters.Add("search", req.Search ?? "", DbType.String);
        parameters.Add("min_departments", req.MinDepartmentCount ?? 0, DbType.Int32);
        parameters.Add("offset", (req.Page - 1) * req.PageSize, DbType.Int32);
        parameters.Add("page_size", req.PageSize, DbType.Int32);

        var sql = $"""
                   WITH filtered AS (
                       SELECT l.id,
                              l.created_at,
                              l.city,
                              l.street,
                              l.house_number,
                              l.number,
                              l.name,
                              COUNT(dp.department_id) AS department_count
                       FROM locations l
                                JOIN department_locations dp ON l.id = dp.location_id
                       WHERE l.name ILIKE '%' || @search || '%'
                       GROUP BY l.id
                       HAVING COUNT(dp.department_id) >= @min_departments
                   )
                   SELECT *,
                          COUNT(*) OVER() AS total_count
                   FROM filtered
                   ORDER BY {sortColumn} {sortDir}
                   LIMIT @page_size OFFSET @offset
                   """;

        var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        var rows = (await connection.QueryAsync<LocationRowDto>(sql, parameters)).ToList();

        return new PagedResult<LocationListItemDto>
        {
            Items = rows.Select(r => new LocationListItemDto(
                r.Id,
                r.Name,
                new AddressDto
                {
                    City = r.City, 
                    Street = r.Street,
                    HouseNumber = r.HouseNumber,
                    Number = r.Number
                },
                r.CreatedAt,
                r.DepartmentCount)).ToList(),
            TotalCount = rows[0].TotalCount,
            Page = req.Page,
            PageSize = req.PageSize,
        };
    }
}
