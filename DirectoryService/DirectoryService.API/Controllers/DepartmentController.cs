using DirectoryService.API.EndpointsResults;
using DirectoryService.API.Extensions;
using DirectoryService.Application.Departments.Create;
using DirectoryService.Application.Departments.Linking;
using DirectoryService.Application.Departments.Unlinking;
using DirectoryService.Application.Departments.Update;
using DirectoryService.Contracts.Request.Department;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.API.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<DepartmentId>> Create(
        [FromBody] CreateDepartmentRequest request,
        [FromServices] CreateDepartmentHandler handler,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(new CreateDepartmentCommand(request), cancellationToken);
    }

    [HttpPatch("{departmentId:guid}")]
    public async Task<EndpointResult<DepartmentId>> Update(
        [FromRoute] Guid departmentId,
        [FromBody] UpdateDepartmentRequest request,
        [FromServices] UpdateDepartmentHandler handler,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(new UpdateDepartmentCommand(departmentId, request), cancellationToken);
    }

    [HttpPost("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<EndpointResult<DepartmentLocationId>> LinkingLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        [FromServices] LinkingLocationHandler handler,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(new LinkingLocationCommand(departmentId, locationId), cancellationToken);
    }

    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<EndpointResult<DepartmentId>> UnlinkingLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        [FromServices] UnlinkingLocationHandler handler,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(new UnlinkingLocationCommand(departmentId, locationId), cancellationToken);
    }

    [HttpGet]
    public Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        return Task.FromResult<ActionResult>(Ok("Departments list"));
    }

    [HttpGet("{departmentId:guid}")]
    public Task<ActionResult> GetById([FromRoute] Guid departmentId, CancellationToken cancellationToken)
    {
        return Task.FromResult<ActionResult>(Ok($"Department {departmentId}"));
    }
}
