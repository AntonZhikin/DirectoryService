using DirectoryService.Contracts.Dtos;

namespace DirectoryService.Contracts.Response.Location;
////https:/t.me/+2WwwviP41mZkODQ6
public record LocationDto(
    Guid Id,
    string Name,
    string TimeZone,
    AddressDto Address);
