using CSharpFunctionalExtensions;
using DirectoryService.Shared.ErrorManagement;
using MediatR;

namespace DirectoryService.Application.Abstraction;

public interface IQuery<TResponse> : IRequest<Result<TResponse, AppError>>;

public interface IQueryHandler<TResponse, in TQuery> : IRequestHandler<TQuery, Result<TResponse, AppError>>
    where TQuery : IQuery<TResponse>;
