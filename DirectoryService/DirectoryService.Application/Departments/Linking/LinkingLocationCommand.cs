using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.DepartmentLocations;

namespace DirectoryService.Application.Departments.Linking;

public record LinkingLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand<DepartmentLocationId>;