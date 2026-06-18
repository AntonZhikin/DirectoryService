using DirectoryService.API.EndpointsResults;
using DirectoryService.Application.Departments.Create;
using DirectoryService.Application.Departments.Delete;
using DirectoryService.Application.Departments.Linking;
using DirectoryService.Application.Departments.LinkingPosition;
using DirectoryService.Application.Departments.Unlinking;
using DirectoryService.Application.Departments.UnlinkingPosition;
using DirectoryService.Application.Departments.Update;
using DirectoryService.Contracts.Request.Department;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.API.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<DepartmentId>> Create(
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateDepartmentCommand(request), cancellationToken);
        return EndpointResult<DepartmentId>.Created(result);
    }

    [HttpPatch("{departmentId:guid}")]
    public async Task<EndpointResult<DepartmentId>> Update(
        [FromRoute] Guid departmentId,
        [FromBody] UpdateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new UpdateDepartmentCommand(departmentId, request), cancellationToken);
    }

    [HttpDelete("{departmentId:guid}")]
    public async Task<EndpointResult<DepartmentId>> Delete(
        [FromRoute] Guid departmentId,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new DeleteDepartmentCommand(departmentId), cancellationToken);
    }

    [HttpPost("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<EndpointResult<DepartmentLocationId>> LinkingLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new LinkingLocationCommand(departmentId, locationId), cancellationToken);
        return EndpointResult<DepartmentLocationId>.Created(result);
    }

    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<EndpointResult<DepartmentId>> UnlinkingLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new UnlinkingLocationCommand(departmentId, locationId), cancellationToken);
    }

    [HttpPost("{departmentId:guid}/positions/{positionId:guid}")]
    public async Task<EndpointResult<DepartmentPositionId>> LinkingPosition(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid positionId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new LinkingPositionCommand(departmentId, positionId), cancellationToken);
        return EndpointResult<DepartmentPositionId>.Created(result);
    }

    [HttpDelete("{departmentId:guid}/positions/{positionId:guid}")]
    public async Task<EndpointResult<DepartmentId>> UnlinkingPosition(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid positionId,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new UnlinkingPositionCommand(departmentId, positionId), cancellationToken);
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
