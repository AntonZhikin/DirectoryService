using DirectoryService.API.EndpointsResults;
using DirectoryService.Application.Locations.Commands.Create;
using DirectoryService.Application.Locations.Commands.Delete;
using DirectoryService.Application.Locations.Commands.Update;
using DirectoryService.Application.Locations.Queries.GetTop;
using DirectoryService.Application.Locations.Queries.GetById;
using DirectoryService.Application.Locations.Queries.GetList;
using DirectoryService.Contracts.Request.Location;
using DirectoryService.Contracts.Response;
using DirectoryService.Contracts.Response.Location;
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

    [HttpGet("top")]
    public async Task<EndpointResult<List<LocationTopDto>>> GetTop(CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetTopLocationsQuery(), cancellationToken);
    }

    [HttpGet("{locationId:guid}")]
    public async Task<EndpointResult<LocationDto>> GetById(
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetLocationByIdQuery(locationId), cancellationToken);
    }
    
    [HttpGet]
    public async Task<EndpointResult<PagedResult<LocationListItemDto>>> Get(
        [FromQuery] GetLocationsRequest request,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetLocationsQuery(request), cancellationToken);
    }
}
