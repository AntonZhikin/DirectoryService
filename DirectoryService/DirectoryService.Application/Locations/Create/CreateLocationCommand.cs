using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Request.Location;

namespace DirectoryService.Application.Locations.Create;

public record CreateLocationCommand(CreateLocationRequest Request) : ICommand;