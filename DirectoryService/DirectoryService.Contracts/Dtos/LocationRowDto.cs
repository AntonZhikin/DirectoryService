namespace DirectoryService.Contracts.Dtos;

public record LocationRowDto(
    Guid Id,
    string Name,
    string City,
    string Street,
    string HouseNumber,
    string Number,
    DateTime CreatedAt,
    int DepartmentCount,
    int TotalCount);