using DirectoryService.API.EndpointsResults;
using DirectoryService.Application.Positions.Create;
using DirectoryService.Application.Positions.Delete;
using DirectoryService.Application.Positions.Update;
using DirectoryService.Contracts.Request.Position;
using DirectoryService.Domain.Positions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.API.Controllers;

[ApiController]
[Route("api/positions")]
public class PositionController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<PositionId>> Create(
        [FromBody] CreatePositionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreatePositionCommand(request), cancellationToken);
        return EndpointResult<PositionId>.Created(result);
    }

    [HttpPatch("{positionId:guid}")]
    public async Task<EndpointResult<PositionId>> Update(
        [FromRoute] Guid positionId,
        [FromBody] UpdatePositionRequest request,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new UpdatePositionCommand(positionId, request), cancellationToken);
    }

    [HttpDelete("{positionId:guid}")]
    public async Task<EndpointResult<PositionId>> Delete(
        [FromRoute] Guid positionId,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new DeletePositionCommand(positionId), cancellationToken);
    }

    [HttpGet]
    public Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        return Task.FromResult<ActionResult>(Ok("Positions list"));
    }

    [HttpGet("{positionId:guid}")]
    public Task<ActionResult> GetById([FromRoute] Guid positionId, CancellationToken cancellationToken)
    {
        return Task.FromResult<ActionResult>(Ok($"Position {positionId}"));
    }
}
