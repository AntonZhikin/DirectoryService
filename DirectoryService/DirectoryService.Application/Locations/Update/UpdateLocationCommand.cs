using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Request.Location;

namespace DirectoryService.Application.Locations.Update;

public record UpdateLocationCommand(Guid LocationId, UpdateLocationRequest Request) : ICommand;