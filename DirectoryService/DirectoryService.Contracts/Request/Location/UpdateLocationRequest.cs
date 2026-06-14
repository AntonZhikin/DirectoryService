using DirectoryService.Contracts.Dtos;

namespace DirectoryService.Contracts.Request.Location;

public record UpdateLocationRequest(string Name, string TimeZone, AddressDto Address, bool IsActive);
