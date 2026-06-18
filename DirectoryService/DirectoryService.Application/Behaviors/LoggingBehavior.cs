using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string commandName = typeof(TRequest).Name;

        logger.LogInformation("Starting command {CommandName}", commandName);

        var response = await next(cancellationToken);

        if (response is IResult { IsFailure: true })
            logger.LogWarning("Command {CommandName} completed with failure", commandName);
        else
            logger.LogInformation("Command {CommandName} handled successfully", commandName);

        return response;
    }
}
