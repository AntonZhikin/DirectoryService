using Dapper;
using DirectoryService.Application.Database;

namespace DirectoryService.Infrastructure.BackgroundServices;

public class SoftDeleteCleaner(IDbConnectionFactory connectionFactory)
{
    private const string DELETE_DEPARTMENTS_BATCH =
        """
        DELETE FROM departments
        WHERE id IN (
            SELECT d.id
            FROM departments d
            WHERE d.is_deleted = true
              AND d.deleted_at < @deleted_before
              AND NOT EXISTS (SELECT 1 FROM departments c WHERE c.parent_id = d.id)
            ORDER BY d.deleted_at
            LIMIT @batch_size
            FOR UPDATE SKIP LOCKED)
        """;

    private const string DELETE_LOCATIONS_BATCH =
        """
        DELETE FROM locations
        WHERE id IN (
            SELECT l.id
            FROM locations l
            WHERE l.is_deleted = true
              AND l.deleted_at < @deleted_before
            ORDER BY l.deleted_at
            LIMIT @batch_size
            FOR UPDATE SKIP LOCKED)
        """;

    private const string DELETE_POSITIONS_BATCH =
        """
        WITH batch AS (
            SELECT p.id
            FROM position p
            WHERE p.is_deleted = true
              AND p.deleted_at < @deleted_before
            ORDER BY p.deleted_at
            LIMIT @batch_size
            FOR UPDATE SKIP LOCKED
        ), deleted_links AS (
            DELETE FROM department_positions
            WHERE position_id IN (SELECT id FROM batch)
        )
        DELETE FROM position
        WHERE id IN (SELECT id FROM batch)
        """;

    public async Task<int> CleanupAsync(DateTime deletedBefore, int batchSize, CancellationToken cancellationToken)
    {
        int total = 0;

        total += await DeleteInBatchesAsync(DELETE_DEPARTMENTS_BATCH, deletedBefore, batchSize, cancellationToken);
        total += await DeleteInBatchesAsync(DELETE_LOCATIONS_BATCH, deletedBefore, batchSize, cancellationToken);
        total += await DeleteInBatchesAsync(DELETE_POSITIONS_BATCH, deletedBefore, batchSize, cancellationToken);

        return total;
    }

    private async Task<int> DeleteInBatchesAsync(
        string sql,
        DateTime deletedBefore,
        int batchSize,
        CancellationToken cancellationToken)
    {
        int total = 0;

        while (true)
        {
            using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

            int deleted = await connection.ExecuteAsync(new CommandDefinition(
                sql,
                new { deleted_before = deletedBefore, batch_size = batchSize },
                cancellationToken: cancellationToken));

            if (deleted == 0)
                return total;

            total += deleted;
        }
    }
}
