using DirectoryService.API.EndpointsResults;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Locations.Create;
using DirectoryService.Application.Locations.Update;
using DirectoryService.Contracts.Request.Location;
using DirectoryService.Domain.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.API.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<LocationId>> Create(
        [FromBody] CreateLocationRequest request,
        [FromServices] ICommandHandler<LocationId, CreateLocationCommand> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new CreateLocationCommand(request), cancellationToken);
        return EndpointResult<LocationId>.Created(result);
    }
    
    [HttpPatch("{locationId:guid}")]
    public async Task<EndpointResult<LocationId>> Update(
        [FromRoute] Guid locationId,
        [FromBody] UpdateLocationRequest request,
        [FromServices] ICommandHandler<LocationId, UpdateLocationCommand> handler,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(new UpdateLocationCommand(locationId, request), cancellationToken);
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

    [HttpDelete("{locationId:guid}")]
    public Task<ActionResult> Delete([FromRoute] Guid locationId, CancellationToken cancellationToken)
    {
        return Task.FromResult<ActionResult>(Ok($"Location {locationId} deleted"));
    }
}
