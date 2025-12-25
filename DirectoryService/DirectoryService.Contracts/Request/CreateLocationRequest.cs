namespace DirectoryService.Contracts.Request;

public record CreateLocationRequest(string TimeZone, string Name, string City, string Street, string HouseNumber, string Number);