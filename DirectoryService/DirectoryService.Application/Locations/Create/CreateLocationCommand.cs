using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Request.Location;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Application.Locations.Create;

public record CreateLocationCommand(CreateLocationRequest Request) : ICommand<LocationId>;