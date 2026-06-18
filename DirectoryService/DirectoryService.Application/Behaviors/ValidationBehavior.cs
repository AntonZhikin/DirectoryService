using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Validation;
using FluentValidation;
using MediatR;

namespace DirectoryService.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationFailures = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var errors = validationFailures
            .Where(result => !result.IsValid)
            .ToList();

        if (errors.Count != 0)
        {
            var failure = errors.ToErrors();
            return failure.ToFailureResponse<TResponse>();
        }

        var response = await next(cancellationToken);

        return response;
    }
}
