using DirectoryService.Contracts.Request.Position;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.API.Controllers;

[ApiController]
[Route("api/positions")]
public class PositionController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreatePositionRequest request, CancellationToken cancellationToken)
    {
        return Ok("Position created");
    }

    [HttpGet]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        return Ok("Positions list");
    }

    [HttpGet("{positionId:guid}")]
    public async Task<ActionResult> GetById([FromRoute] Guid positionId, CancellationToken cancellationToken)
    {
        return Ok($"Position {positionId}");
    }

    [HttpPut("{positionId:guid}")]
    public async Task<ActionResult> Update([FromRoute] Guid positionId, [FromBody] UpdatePositionRequest request, CancellationToken cancellationToken)
    {
        return Ok($"Position {positionId} updated");
    }

    [HttpDelete("{positionId:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid positionId, CancellationToken cancellationToken)
    {
        return Ok($"Position {positionId} deleted");
    }
}
