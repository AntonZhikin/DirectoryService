using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Request.Location;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Application.Locations.Commands.Create;

public record CreateLocationCommand(CreateLocationRequest Request) : ICommand<LocationId>;