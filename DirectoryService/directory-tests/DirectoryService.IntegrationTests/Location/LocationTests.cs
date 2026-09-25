using DirectoryService.Application.Locations.Commands.Create;
using DirectoryService.Application.Locations.Commands.Delete;
using DirectoryService.Application.Locations.Commands.Update;
using DirectoryService.Application.Locations.Queries.GetById;
using DirectoryService.Application.Locations.Queries.GetList;
using DirectoryService.Application.Locations.Queries.GetTop;
using DirectoryService.Contracts.Dtos;
using DirectoryService.Contracts.Request.Location;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.IntegrationTests.Abstraction;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.EntityFrameworkCore;
using TimeZone = DirectoryService.Domain.Locations.ValueObjects.TimeZone;

namespace DirectoryService.IntegrationTests.Location;

public class LocationTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    [Fact]
    public async Task CreateLocation_With_Valid_Data_Should_Succeed()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateLocationCommand(new CreateLocationRequest(
                "Главный офис",
                "Europe/Moscow",
                CreateAddress()));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var location = await dbContext.Locations
                .FirstAsync(l => l.Id == result.Value, cancellationToken);

            Assert.Equal("Главный офис", location.Name.Value);
            Assert.Equal("Europe/Moscow", location.TimeZone.Value);
            Assert.Equal("москва", location.Address.City);
            Assert.True(location.IsActive);
        });
    }

    [Fact]
    public async Task CreateLocation_With_Invalid_TimeZone_Should_Return_Validation_Error()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateLocationCommand(new CreateLocationRequest(
                "Главный офис",
                "Mars/Olympus",
                CreateAddress()));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.VALIDATION, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            Assert.Equal(0, await dbContext.Locations.CountAsync(cancellationToken));
        });
    }

    [Fact]
    public async Task CreateLocation_With_Existing_Name_Should_Return_Conflict()
    {
        await CreateLocation("Главный офис");

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateLocationCommand(new CreateLocationRequest(
                "Главный офис",
                "Europe/Moscow",
                CreateAddress()));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.CONFLICT, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            Assert.Equal(1, await dbContext.Locations.CountAsync(cancellationToken));
        });
    }

    [Fact]
    public async Task UpdateLocation_With_Valid_Data_Should_Succeed()
    {
        LocationId locationId = await CreateLocation("Старый офис");

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new UpdateLocationCommand(locationId.Value, new UpdateLocationRequest(
                "Новый офис",
                "Asia/Yekaterinburg",
                CreateAddress(),
                false));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var location = await dbContext.Locations
                .FirstAsync(l => l.Id == locationId, cancellationToken);

            Assert.Equal("Новый офис", location.Name.Value);
            Assert.Equal("Asia/Yekaterinburg", location.TimeZone.Value);
            Assert.False(location.IsActive);
        });
    }

    [Fact]
    public async Task UpdateLocation_With_Not_Existing_Id_Should_Return_NotFound()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new UpdateLocationCommand(Guid.NewGuid(), new UpdateLocationRequest(
                "Новый офис",
                "Europe/Moscow",
                CreateAddress(),
                true));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NOT_FOUND, result.Error.Type);
    }

    [Fact]
    public async Task UpdateLocation_With_Invalid_TimeZone_Should_Return_Validation_Error()
    {
        LocationId locationId = await CreateLocation("Старый офис");

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new UpdateLocationCommand(locationId.Value, new UpdateLocationRequest(
                "Новый офис",
                "Mars/Olympus",
                CreateAddress(),
                true));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.VALIDATION, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            var location = await dbContext.Locations
                .FirstAsync(l => l.Id == locationId, cancellationToken);

            Assert.Equal("Старый офис", location.Name.Value);
        });
    }

    [Fact]
    public async Task DeleteLocation_Without_Departments_Should_Succeed()
    {
        LocationId locationId = await CreateLocation("Главный офис");

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new DeleteLocationCommand(locationId.Value), cancellationToken));

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            Assert.False(await dbContext.Locations.AnyAsync(l => l.Id == locationId, cancellationToken));
        });
    }

    [Fact]
    public async Task DeleteLocation_With_Linked_Department_Should_Return_Conflict()
    {
        LocationId locationId = await CreateLocation("Главный офис");
        await CreateDepartment("Отдел продаж", "sales", locationId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new DeleteLocationCommand(locationId.Value), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.CONFLICT, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            Assert.True(await dbContext.Locations.AnyAsync(l => l.Id == locationId, cancellationToken));
        });
    }

    [Fact]
    public async Task DeleteLocation_With_Not_Existing_Id_Should_Return_NotFound()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new DeleteLocationCommand(Guid.NewGuid()), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NOT_FOUND, result.Error.Type);
    }

    [Fact]
    public async Task GetLocationById_With_Existing_Id_Should_Return_Location()
    {
        LocationId locationId = await CreateLocation("Главный офис");

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new GetLocationByIdQuery(locationId.Value), cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(locationId.Value, result.Value.Id);
        Assert.Equal("Главный офис", result.Value.Name);
        Assert.Equal("Europe/Moscow", result.Value.TimeZone);
        Assert.Equal("москва", result.Value.Address.City);
    }

    [Fact]
    public async Task GetLocationById_With_Not_Existing_Id_Should_Return_NotFound()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new GetLocationByIdQuery(Guid.NewGuid()), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NOT_FOUND, result.Error.Type);
    }

    [Fact]
    public async Task GetLocations_With_Search_And_MinDepartmentCount_Should_Filter()
    {
        LocationId moscowOffice = await CreateLocation("Moscow Office");
        LocationId moscowWarehouse = await CreateLocation("Moscow Warehouse");
        await CreateLocation("Berlin Office");
        await CreateDepartment("Отдел продаж", "sales", moscowOffice);
        await CreateDepartment("Бухгалтерия", "accounting", moscowOffice);
        await CreateDepartment("Склад", "warehouse", moscowWarehouse);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new GetLocationsQuery(new GetLocationsRequest("moscow", null, null, 2)), cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.TotalCount);

        var location = Assert.Single(result.Value.Items);
        Assert.Equal(moscowOffice.Value, location.Id);
        Assert.Equal(2, location.DepartmentCount);
    }

    [Fact]
    public async Task GetLocations_With_Invalid_PageSize_Should_Return_Validation_Error()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new GetLocationsQuery(new GetLocationsRequest(null, null, null, null, 0)), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.VALIDATION, result.Error.Type);
    }

    [Fact]
    public async Task GetTopLocations_Should_Return_Only_Linked_Locations_Ordered_By_Department_Count()
    {
        LocationId moscowOffice = await CreateLocation("Moscow Office");
        LocationId moscowWarehouse = await CreateLocation("Moscow Warehouse");
        await CreateLocation("Berlin Office");
        await CreateDepartment("Отдел продаж", "sales", moscowOffice);
        await CreateDepartment("Бухгалтерия", "accounting", moscowOffice);
        await CreateDepartment("Склад", "warehouse", moscowWarehouse);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new GetTopLocationsQuery(), cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal(moscowOffice.Value, result.Value[0].Id);
        Assert.Equal(2, result.Value[0].DepartmentCount);
        Assert.Equal("москва", result.Value[0].Address.City);
        Assert.Equal(moscowWarehouse.Value, result.Value[1].Id);
    }

    [Fact]
    public async Task GetTopLocations_Without_Linked_Locations_Should_Return_Empty_List()
    {
        await CreateLocation("Berlin Office");

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new GetTopLocationsQuery(), cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private static AddressDto CreateAddress() => new()
    {
        City = "москва",
        Street = "пушкина",
        HouseNumber = "12",
        Number = "10",
    };

    private async Task<LocationId> CreateLocation(string name)
    {
        return await ExecuteInDb(async dbContext =>
        {
            var location = new Domain.Locations.Location(
                LocationName.Create(name).Value,
                TimeZone.Create("Europe/Moscow").Value,
                new Address("москва", "пушкина", "12", "10"));

            dbContext.Add(location);

            await dbContext.SaveChangesAsync();

            return location.Id;
        });
    }

    private async Task<DepartmentId> CreateDepartment(string name, string slug, LocationId locationId)
    {
        return await ExecuteInDb(async dbContext =>
        {
            var departmentId = new DepartmentId(Guid.NewGuid());

            List<DepartmentLocation> departmentLocations =
            [
                new()
                {
                    Id = new DepartmentLocationId(Guid.NewGuid()),
                    LocationId = locationId,
                    DepartmentId = departmentId,
                    IsPrimary = false,
                },
            ];

            var department = Domain.Departments.Department.CreateParent(
                DepartmentName.Create(name).Value,
                Identifier.Create(slug).Value,
                departmentLocations,
                departmentId).Value;

            dbContext.Add(department);

            await dbContext.SaveChangesAsync();

            return department.Id;
        });
    }
}
