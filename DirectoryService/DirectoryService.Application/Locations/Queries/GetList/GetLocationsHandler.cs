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
    public async Task<Result<PagedResult<LocationListItemDto>, AppError>> Handle(
        GetLocationsQuery query, CancellationToken cancellationToken)
    {
        var req = query.Request;

        if (req.PageSize is < 1 or > 100)
            return AppErrors.ValueIsInvalid("pageSize");
        
        if (req.Search?.Length > 100)
            return AppErrors.ValueIsInvalid("search");

        string sortDir = req.SortDir?.ToLower() == "desc" ? "DESC" : "ASC";
        string sortColumn = req.SortBy?.ToLower() switch
        {
            "name" => "name",
            "createdat" => "created_at",
            "departmentcount" => "department_count",
            _ => "name",
        };

        string orderByClause = $"ORDER BY {sortColumn} {sortDir}";

        var parameters = new DynamicParameters();
        
        parameters.Add("search", req.Search ?? "", DbType.String);
        parameters.Add("min_departments", req.MinDepartmentCount ?? 0, DbType.Int32);
        parameters.Add("offset", (req.Page - 1) * req.PageSize, DbType.Int32);
        parameters.Add("page_size", req.PageSize, DbType.Int32);

        string sql = $"""
                      WITH filtered AS (
                          SELECT l.id,
                                 l.name,
                                 l.city,
                                 l.street,
                                 l.house_number,
                                 l.number,
                                 l.created_at,
                                 COUNT(dl.department_id) AS department_count
                          FROM locations l
                                   LEFT JOIN department_locations dl ON l.id = dl.location_id
                          WHERE l.name ILIKE '%' || @search || '%'
                          GROUP BY l.id
                          HAVING COUNT(dl.department_id) >= @min_departments
                      )
                      SELECT *,
                             COUNT(*) OVER() AS total_count
                      FROM filtered
                      {orderByClause}
                      LIMIT @page_size OFFSET @offset
                      """;

        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        var rows = (await connection.QueryAsync<LocationRowDto>(sql, parameters)).ToList();

        var locations = rows.Select(r => new LocationListItemDto(
            r.Id,
            r.Name,
            new AddressDto
            {
                City = r.City, Street = r.Street, HouseNumber = r.HouseNumber, Number = r.Number,
            },
            r.CreatedAt,
            (int)r.DepartmentCount)).ToList();

        return new PagedResult<LocationListItemDto>
        {
            Items = locations,
            TotalCount = (int)(rows.FirstOrDefault()?.TotalCount ?? 0),
            Page = req.Page,
            PageSize = req.PageSize,
        };
    }
}
