using DirectoryService.Contracts.Dtos;

namespace DirectoryService.Contracts.Response.Location;

public record LocationTopDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public AddressDto Address { get; init; }
    public int DepartmentCount { get; init; }
}
    