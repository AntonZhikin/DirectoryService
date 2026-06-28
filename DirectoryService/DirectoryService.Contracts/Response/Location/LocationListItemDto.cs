using DirectoryService.Contracts.Dtos;

namespace DirectoryService.Contracts.Response.Location;

public record LocationListItemDto(
    Guid Id,
    string Name,
    AddressDto Address,
    DateTime CreatedAt,
    int DepartmentCount);