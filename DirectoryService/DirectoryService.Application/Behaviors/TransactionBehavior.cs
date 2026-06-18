using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database.Transaction;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(
    ITransactionManager transactionManager,
    ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string commandName = typeof(TRequest).Name;

        var transactionResult = await transactionManager.BeginTransaction(cancellationToken);
        if (transactionResult.IsFailure)
            return transactionResult.Error.ToFailureResponse<TResponse>();

        using var transaction = transactionResult.Value;

        var response = await next(cancellationToken);

        if (response is IResult { IsFailure: true })
        {
            transaction.Rollback();
            logger.LogWarning("Transaction rolled back for {CommandName}", commandName);
            return response;
        }

        var saveResult = await transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            transaction.Rollback();
            logger.LogError("SaveChanges failed for {CommandName}, transaction rolled back", commandName);
            return saveResult.Error.ToFailureResponse<TResponse>();
        }

        var commitResult = transaction.Commit();
        if (commitResult.IsFailure)
        {
            logger.LogError("Commit failed for {CommandName}", commandName);
            return commitResult.Error.ToFailureResponse<TResponse>();
        }

        return response;
    }
}
