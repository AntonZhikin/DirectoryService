using CSharpFunctionalExtensions;
using DirectoryService.Shared.ErrorManagement;

namespace DirectoryService.Domain.Location.ValueObjects;

public record Address
{
    //EF Core
    private Address() { }

    public string City { get; } = null!;
    public string Street { get; } = null!;

    public string HouseNumber { get; } = null!;

    public string Number { get; }

    public string FullAddress(string locationName) => $"{locationName} {City} {Street} {HouseNumber} {Number}";

    public Address(
        string city,
        string street,
        string houseNumber,
        string number)
    {
        City = city;
        Street = street;
        HouseNumber = houseNumber;
        Number = number;
    }

    public static Result<Address, AppError> Create(string city, string street, string houseNumber, string number)
    {
        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(houseNumber) || string.IsNullOrWhiteSpace(number))
        {
            return AppErrors.ValueIsInvalid("Address");
        }
        
        return new Address(city, street, houseNumber, number);
    }
}