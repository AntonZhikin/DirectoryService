using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Location;
using DirectoryService.Infrastructure.Database;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Infrastructure.Repositories;

public class DapperLocationRepository(IDbConnectionFactory connectionFactory) : ILocationRepository
{
    public async Task<Result<LocationId, AppErrorList>> AddAsync(Location location, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO locations (id, name, timezone, city, street, house_number, number, is_active, created_at, updated_at)
            VALUES (@Id, @Name, @TimeZone, @City, @Street, @HouseNumber, @Number, @IsActive, @CreatedAt, @UpdatedAt)
            """;

        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(sql, new
        {
            Id          = location.Id.Value,
            Name        = location.Name.Value,
            TimeZone    = location.TimeZone.Value,
            City        = location.Address.City,
            Street      = location.Address.Street,
            HouseNumber = location.Address.HouseNumber,
            Number      = location.Address.Number,
            IsActive    = location.IsActive,
            CreatedAt   = location.CreatedAt,
            UpdatedAt   = location.UpdatedAt,
        });

        return location.Id;
    }

    public async Task<bool> IsNameTakenAsync(string name, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM locations WHERE name = @Name";

        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        
        int count = await connection.ExecuteScalarAsync<int>(sql, new { Name = name });
        return count > 0;
    }
}
