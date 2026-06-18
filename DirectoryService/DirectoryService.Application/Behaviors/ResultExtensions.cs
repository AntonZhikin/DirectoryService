using CSharpFunctionalExtensions;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Application.Behaviors;

public static class ResultExtensions
{
    public static TResponse ToFailureResponse<TResponse>(this AppError error)
    {
        var successType = typeof(TResponse).GetGenericArguments()[0];
        var failureType = typeof(TResponse).GetGenericArguments()[1];

        var failureMethod = typeof(Result)
            .GetMethods()
            .First(m => m.Name == "Failure" &&
                        m.GetParameters().Length == 1 &&
                        m.GetGenericArguments().Length == 2)
            .MakeGenericMethod(successType, failureType);

        return (TResponse)failureMethod.Invoke(null, [error])!;
    }
}
