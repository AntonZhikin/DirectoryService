using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.DepartmentLocations;

namespace DirectoryService.Application.Departments.Commands.Linking;

public record LinkingLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand<DepartmentLocationId>;