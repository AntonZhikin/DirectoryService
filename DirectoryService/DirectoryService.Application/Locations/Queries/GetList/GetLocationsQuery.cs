using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Request.Location;
using DirectoryService.Contracts.Response;
using DirectoryService.Contracts.Response.Location;

namespace DirectoryService.Application.Locations.Queries.GetList;

public record GetLocationsQuery(GetLocationsRequest Request) 
    : IQuery<PagedResult<LocationListItemDto>>;