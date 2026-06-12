using DirectoryService.Contracts.Dtos;

namespace DirectoryService.Contracts.Request.Location;

public record CreateLocationRequest(string Name, string TimeZone, AddressDto Address);
