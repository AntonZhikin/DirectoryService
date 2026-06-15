using DirectoryService.Application.Departments;
using DirectoryService.Application.Departments.Create;
using DirectoryService.Application.Departments.Linking;
using DirectoryService.Application.Departments.Unlinking;
using DirectoryService.Application.Departments.Update;
using DirectoryService.Contracts.Request.Department;
using DirectoryService.Shared.Response;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.API.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(
        [FromBody] CreateDepartmentRequest request,
        [FromServices] CreateDepartmentHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateDepartmentCommand(request);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(Envelope.Ok(result.Value));
    }
    
    [HttpPatch("{departmentId:guid}")]
    public async Task<ActionResult> Update(
        [FromRoute] Guid departmentId,
        [FromBody] UpdateDepartmentRequest request,
        [FromServices] UpdateDepartmentHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDepartmentCommand(departmentId, request);
        
        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return BadRequest(result.Error);
        
        return Ok(Envelope.Ok(result.Value));
    }
    
    [HttpPost("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<ActionResult> LinkingLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        [FromServices] LinkingLocationHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new LinkingLocationCommand(departmentId, locationId);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(Envelope.Ok(result.Value));
    }
    
    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<ActionResult> UnlinkingLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        [FromServices] UnlinkingLocationHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new UnlinkingLocationCommand(departmentId, locationId);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(Envelope.Ok(result.Value));
    }

    [HttpGet]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        return Ok("Departments list");
    }

    [HttpGet("{departmentId:guid}")]
    public async Task<ActionResult> GetById([FromRoute] Guid departmentId, CancellationToken cancellationToken)
    {
        return Ok($"Department {departmentId}");
    }

    [HttpDelete("{departmentId:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid departmentId, CancellationToken cancellationToken)
    {
        return Ok($"Department {departmentId} deleted");
    }
}
