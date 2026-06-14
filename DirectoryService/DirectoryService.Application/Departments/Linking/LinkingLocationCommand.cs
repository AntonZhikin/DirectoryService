namespace DirectoryService.Application.Departments.Linking;

public record LinkingLocationCommand(Guid DepartmentId, Guid LocationId);