using DirectoryService.API.EndpointsResults;
using DirectoryService.Application.Locations.Create;
using DirectoryService.Application.Locations.Delete;
using DirectoryService.Application.Locations.Update;
using DirectoryService.Contracts.Request.Location;
using DirectoryService.Domain.Locations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.API.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<LocationId>> Create(
        [FromBody] CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateLocationCommand(request), cancellationToken);
        return EndpointResult<LocationId>.Created(result);
    }

    [HttpPatch("{locationId:guid}")]
    public async Task<EndpointResult<LocationId>> Update(
        [FromRoute] Guid locationId,
        [FromBody] UpdateLocationRequest request,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new UpdateLocationCommand(locationId, request), cancellationToken);
    }

    [HttpDelete("{locationId:guid}")]
    public async Task<EndpointResult<LocationId>> Delete(
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new DeleteLocationCommand(locationId), cancellationToken);
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
}
