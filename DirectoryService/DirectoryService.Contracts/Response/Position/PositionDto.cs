namespace DirectoryService.Contracts.Response.Position;

public record PositionDto(
    Guid Id,
    string Name,
    string Description);
