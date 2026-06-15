using DirectoryService.Contracts.Request.Position;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.API.Controllers;

[ApiController]
[Route("api/positions")]
public class PositionController : ControllerBase
{
    [HttpPost]
    public Task<ActionResult> Create([FromBody] CreatePositionRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult<ActionResult>(Ok("Position created"));
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

    [HttpPut("{positionId:guid}")]
    public Task<ActionResult> Update([FromRoute] Guid positionId, [FromBody] UpdatePositionRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult<ActionResult>(Ok($"Position {positionId} updated"));
    }

    [HttpDelete("{positionId:guid}")]
    public Task<ActionResult> Delete([FromRoute] Guid positionId, CancellationToken cancellationToken)
    {
        return Task.FromResult<ActionResult>(Ok($"Position {positionId} deleted"));
    }
}
