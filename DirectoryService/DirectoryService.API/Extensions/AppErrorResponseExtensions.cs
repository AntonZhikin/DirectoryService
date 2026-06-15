using DirectoryService.Shared.ErrorManagement;
using DirectoryService.Shared.Response;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.API.Extensions;

public static class AppErrorResponseExtensions
{
    private static int ToStatusCode(this ErrorType type) => type switch
    {
        ErrorType.VALIDATION  => StatusCodes.Status400BadRequest,
        ErrorType.NOT_FOUND   => StatusCodes.Status404NotFound,
        ErrorType.CONFLICT    => StatusCodes.Status409Conflict,
        ErrorType.EXTERNAL    => StatusCodes.Status502BadGateway,
        ErrorType.FAILURE     => StatusCodes.Status500InternalServerError,
        ErrorType.UN_EXPECTED => StatusCodes.Status500InternalServerError,
        _                     => StatusCodes.Status500InternalServerError,
    };

    public static ActionResult ToActionResult(this AppError error) =>
        new ObjectResult(Envelope.Error(error)) { StatusCode = error.Type.ToStatusCode() };
}
