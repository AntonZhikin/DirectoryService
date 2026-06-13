using DirectoryService.Application.Departments;
using DirectoryService.Contracts.Request.Department;
using DirectoryService.Contracts.Response.Department;
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

    [HttpPut("{departmentId:guid}")]
    public async Task<ActionResult> Update(
        [FromRoute] Guid departmentId,
        [FromBody] UpdateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        return Ok($"Department {departmentId} updated");
    }

    [HttpDelete("{departmentId:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid departmentId, CancellationToken cancellationToken)
    {
        return Ok($"Department {departmentId} deleted");
    }
}
