using System.Data;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Database;

public class TransactionScope(IDbTransaction transaction, ILogger<TransactionScope> logger)
    : ITransactionScope
{
    public UnitResult<AppError> Commit()
    {
        try
        {
            transaction.Commit();
            return UnitResult.Success<AppError>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to commit transaction");
            return AppError.Failure("transaction.commit.failed", "Failed to commit transaction");
        }
    }
    
    public UnitResult<AppError> Rollback()
    {
        try
        {
            transaction.Rollback();
            return UnitResult.Success<AppError>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to rollback transaction");
            return AppError.Failure("transaction.rollback.failed", "Failed to rollback transaction");
        }
    }

    public void Dispose()
    {
        transaction.Dispose();
    }
}