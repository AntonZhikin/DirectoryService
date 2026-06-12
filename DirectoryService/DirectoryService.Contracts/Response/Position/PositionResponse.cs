namespace DirectoryService.Contracts.Response.Position;

public record PositionResponse(
    Guid Id,
    string Name,
    string Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
