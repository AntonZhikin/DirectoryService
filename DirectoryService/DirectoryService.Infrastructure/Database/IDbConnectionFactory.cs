using System.Data;

namespace DirectoryService.Infrastructure.Database;

public interface IDbConnectionFactory
{ 
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}