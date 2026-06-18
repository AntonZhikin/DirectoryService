using CSharpFunctionalExtensions;
using DirectoryService.Shared.ErrorManagement;
using MediatR;

namespace DirectoryService.Application.Abstraction;

public interface ICommandHandler<TResponse, in TCommand> : IRequestHandler<TCommand, Result<TResponse, AppError>>
    where TCommand : ICommand<TResponse>;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, UnitResult<AppError>>
    where TCommand : ICommand;
