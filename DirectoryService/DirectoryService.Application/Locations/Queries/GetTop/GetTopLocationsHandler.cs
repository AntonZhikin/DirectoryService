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

        var locationDto = await connection.QueryAsync<LocationTopDto, AddressDto, LocationTopDto>(
            """
            SELECT
                locations.id,
                name,
                COUNT(department_locations.department_id) AS department_count,
                city,
                street,
                house_number,
                number
            FROM locations
            JOIN department_locations ON locations.id = department_locations.location_id
            GROUP BY locations.id, name, city, street, house_number, number
            ORDER BY department_count DESC
            LIMIT 5
            """,
            splitOn: "city",
            map: (location, address) => location with { Address = address });
        
        return locationDto.ToList();
    }
}
