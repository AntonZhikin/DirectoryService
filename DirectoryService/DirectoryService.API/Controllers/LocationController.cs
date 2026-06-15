using DirectoryService.API.Extensions;
using DirectoryService.Application.Locations.Create;
using DirectoryService.Application.Locations.Update;
using DirectoryService.Contracts.Request.Location;
using DirectoryService.Shared.Response;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.API.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(
        [FromBody] CreateLocationRequest request,
        [FromServices] CreateLocationHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(request);

        var result = await handler.Handle(command, cancellationToken);
        return result.IsFailure 
            ? result.Error.ToActionResult() 
            : Ok(Envelope.Ok(result.Value));
    }

    [HttpGet]
    public Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        return Task.FromResult<ActionResult>(Ok("Locations list"));
    }

    [HttpGet("{locationId:guid}")]
    public Task<ActionResult> GetById([FromRoute] Guid locationId, CancellationToken cancellationToken)
    {
        return Task.FromResult<ActionResult>(Ok($"Location {locationId}"));
    }

    [HttpPatch("{locationId:guid}")]
    public async Task<ActionResult> Update(
        [FromRoute] Guid locationId,
        [FromBody] UpdateLocationRequest request,
        [FromServices] UpdateLocationHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationCommand(locationId, request);

        var result = await handler.Handle(command, cancellationToken);
        return result.IsFailure
            ? result.Error.ToActionResult()
            : Ok(Envelope.Ok(result.Value));
    }

    [HttpDelete("{locationId:guid}")]
    public Task<ActionResult> Delete([FromRoute] Guid locationId, CancellationToken cancellationToken)
    {
        return Task.FromResult<ActionResult>(Ok($"Location {locationId} deleted"));
    }
}
