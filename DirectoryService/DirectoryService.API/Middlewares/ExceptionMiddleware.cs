using DirectoryService.Shared.ErrorManagement;
using DirectoryService.Shared.Response;

namespace DirectoryService.API.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Полные детали — только в лог. Наружу — дженерик, чтобы не утекали внутренности.
        logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var envelope = Envelope.Error(AppError.Failure("server.internal", "Произошла внутренняя ошибка сервера"));
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await context.Response.WriteAsJsonAsync(envelope);
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}