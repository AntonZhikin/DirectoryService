namespace DirectoryService.Application.Locations.Dtos;

public record LocationRowDto(
    Guid Id,
    string Name,
    string City,
    string Street,
    string HouseNumber,
    string Number,
    DateTime CreatedAt,
    long DepartmentCount,
    long TotalCount);
