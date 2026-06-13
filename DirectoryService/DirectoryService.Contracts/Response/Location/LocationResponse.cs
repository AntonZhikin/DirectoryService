using DirectoryService.Contracts.Dtos;

namespace DirectoryService.Contracts.Response.Location;

public record LocationResponse(
    Guid Id,
    string Name,
    string TimeZone,
    AddressDto Address,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
