using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Application.Locations.Commands.Delete;

public record DeleteLocationCommand(Guid LocationId) : ICommand<LocationId>;
