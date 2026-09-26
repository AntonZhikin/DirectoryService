using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Dtos;
using DirectoryService.Contracts.Response.Location;
using DirectoryService.Shared.ErrorManagement;
using MediatR;

namespace DirectoryService.Application.Locations.Queries.GetTop;

public class GetTopLocationsHandler(IDbConnectionFactory connectionFactory)
    : IRequestHandler<GetTopLocationsQuery, Result<List<LocationTopDto>, AppError>>
{
    public async Task<Result<List<LocationTopDto>, AppError>> Handle(
        GetTopLocationsQuery query,
        CancellationToken cancellationToken)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var locationsDto = await connection.QueryAsync<LocationTopDto, AddressDto, LocationTopDto>(
            """
            SELECT
                l.id,
                l.name,
                COUNT(d.id) AS department_count,
                l.city,
                l.street,
                l.house_number,
                l.number
            FROM locations l
            JOIN department_locations dl ON l.id = dl.location_id
            JOIN departments d ON d.id = dl.department_id AND d.is_deleted = false
            WHERE l.is_deleted = false
            GROUP BY l.id, l.name, l.city, l.street, l.house_number, l.number
            ORDER BY department_count DESC, l.name ASC
            LIMIT 5
            """,
            splitOn: "city",
            map: (location, address) => location with { Address = address });
        
        return locationsDto.ToList();
    }
}
