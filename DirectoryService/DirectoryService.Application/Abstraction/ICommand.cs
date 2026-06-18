using CSharpFunctionalExtensions;
using DirectoryService.Shared.ErrorManagement;
using MediatR;

namespace DirectoryService.Application.Abstraction;

public interface ICommand<TResponse> : IBaseCommand, IRequest<Result<TResponse, AppError>>;

public interface ICommand : IBaseCommand, IRequest<UnitResult<AppError>>;