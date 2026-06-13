using DirectoryService.Application.Locations;
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
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(Envelope.Ok(result.Value));
    }

    [HttpGet]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        return Ok("Locations list");
    }

    [HttpGet("{locationId:guid}")]
    public async Task<ActionResult> GetById([FromRoute] Guid locationId, CancellationToken cancellationToken)
    {
        return Ok($"Location {locationId}");
    }

    [HttpPut("{locationId:guid}")]
    public async Task<ActionResult> Update(
        [FromRoute] Guid locationId,
        [FromBody] UpdateLocationRequest request,
        CancellationToken cancellationToken)
    {
        return Ok($"Location {locationId} updated");
    }

    [HttpDelete("{locationId:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid locationId, CancellationToken cancellationToken)
    {
        return Ok($"Location {locationId} deleted");
    }
}
