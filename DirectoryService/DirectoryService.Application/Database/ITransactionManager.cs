using CSharpFunctionalExtensions;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Application.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, AppError>> BeginTransaction(CancellationToken cancellationToken);
    Task<UnitResult<AppError>> SaveChangesAsync(CancellationToken cancellationToken);
}