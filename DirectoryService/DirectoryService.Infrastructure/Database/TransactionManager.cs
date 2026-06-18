using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DirectoryService.Infrastructure.Database;

public class TransactionManager(
    ApplicationDbContext dbContext,
    ILogger<TransactionManager> logger,
    ILoggerFactory loggerFactory) : ITransactionManager
{
    public async Task<Result<ITransactionScope, AppError>> BeginTransaction(CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            var newLogger = loggerFactory.CreateLogger<TransactionScope>();
            var transactionScope = new TransactionScope(transaction.GetDbTransaction(), newLogger);

            return transactionScope;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to begin transaction");
            return AppError.Failure("database.transaction", "Failed to begin transaction");
        }
    }

    public async Task<UnitResult<AppError>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<AppError>();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Concurrency conflict during save");
            return AppError.Conflict("database.concurrency",
                "Данные были изменены другим процессом. Повторите операцию.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            return MapPostgresException(pgEx);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database update failed");
            return AppError.Failure("database.error", "Ошибка сохранения данных");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected database error");
            return AppError.Failure("database.error", "Непредвиденная ошибка базы данных");
        }
    }

    private UnitResult<AppError> MapPostgresException(PostgresException pgEx)
    {
        switch (pgEx.SqlState)
        {
            case PostgresErrorCodes.UniqueViolation:
                logger.LogWarning(pgEx, "Unique violation on constraint {Constraint}", pgEx.ConstraintName);
                return AppError.Conflict("database.unique_violation",
                    "Запись с таким значением уже существует", pgEx.ConstraintName);

            case PostgresErrorCodes.ForeignKeyViolation:
                logger.LogWarning(pgEx, "Foreign key violation on constraint {Constraint}", pgEx.ConstraintName);
                return AppError.Conflict("database.foreign_key_violation",
                    "Ссылка на несуществующую или связанную запись", pgEx.ConstraintName);

            default:
                logger.LogError(pgEx, "Postgres error {SqlState}", pgEx.SqlState);
                return AppError.Failure("database.error", "Ошибка сохранения данных");
        }
    }
}