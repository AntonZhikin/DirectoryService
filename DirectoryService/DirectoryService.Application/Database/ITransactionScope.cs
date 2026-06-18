using CSharpFunctionalExtensions;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Application.Database;

public interface ITransactionScope : IDisposable
{
    UnitResult<AppError> Commit();
    UnitResult<AppError> Rollback();
}