using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Request.Location;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Application.Locations.Commands.Update;

public record UpdateLocationCommand(Guid LocationId, UpdateLocationRequest Request) : ICommand<LocationId>;