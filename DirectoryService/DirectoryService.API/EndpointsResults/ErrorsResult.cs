using DirectoryService.API.Extensions;
using DirectoryService.Shared.ErrorManagement;
using DirectoryService.Shared.Response;

namespace DirectoryService.API.EndpointsResults;

public sealed class ErrorsResult(AppError error) : IResult
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        httpContext.Response.StatusCode = error.Type.ToStatusCode();

        return httpContext.Response.WriteAsJsonAsync(Envelope.Error(error));
    }
}
