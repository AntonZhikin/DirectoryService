using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Response.Location;

namespace DirectoryService.Application.Locations.Queries.GetById;

public record GetLocationByIdQuery(Guid LocationId) : IQuery<LocationDto>;
