namespace DirectoryService.Contracts.Dtos;

public record AddressDto
{
    public string City { get; init; }
    public string Street { get; init; }
    public string HouseNumber { get; init; }
    public string Number { get; init; }
};