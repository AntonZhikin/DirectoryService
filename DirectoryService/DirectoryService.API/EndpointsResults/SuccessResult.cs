using DirectoryService.Shared.Response;

namespace DirectoryService.API.EndpointsResults;

public sealed class SuccessResult<TValue>(TValue value, int statusCode = StatusCodes.Status200OK) : IResult
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        httpContext.Response.StatusCode = statusCode;

        return httpContext.Response.WriteAsJsonAsync(Envelope<TValue>.Ok(value));
    }
}
