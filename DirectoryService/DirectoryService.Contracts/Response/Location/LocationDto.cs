using DirectoryService.Contracts.Dtos;

namespace DirectoryService.Contracts.Response.Location;

public record LocationDto(
    Guid Id,
    string Name,
    string TimeZone,
    AddressDto Address);
