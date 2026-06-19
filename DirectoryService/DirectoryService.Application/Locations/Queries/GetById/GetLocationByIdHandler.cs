using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Dtos;
using DirectoryService.Contracts.Response.Location;
using DirectoryService.Domain.Locations;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Application.Locations.Queries.GetById;

public class GetLocationByIdHandler(IReadDbContext readDbContext)
    : IQueryHandler<LocationDto, GetLocationByIdQuery>
{
    public async Task<Result<LocationDto, AppError>> Handle(
        GetLocationByIdQuery query,
        CancellationToken cancellationToken)
    {
        var dto = await readDbContext.LocationsRead
            .Where(l => l.Id == new LocationId(query.LocationId))
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
            return AppErrors.NotFound(query.LocationId, "location");

        return new LocationDto(
            dto.Id.Value,
            dto.Name.Value,
            dto.TimeZone.Value,
            new AddressDto
            {
                City = dto.Address.City,
                Street = dto.Address.Street,
                HouseNumber = dto.Address.HouseNumber,
                Number = dto.Address.Number
            });
        
    }
}
