using DirectoryService.Application.Departments.Commands.Linking;
using DirectoryService.Application.Departments.Commands.LinkingPosition;
using DirectoryService.Application.Departments.Commands.Unlinking;
using DirectoryService.Application.Departments.Commands.UnlinkingPosition;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.IntegrationTests.Abstraction;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.EntityFrameworkCore;
using TimeZone = DirectoryService.Domain.Locations.ValueObjects.TimeZone;

namespace DirectoryService.IntegrationTests.Department;

public class DepartmentLinkingTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    [Fact]
    public async Task LinkingLocation_With_Not_Linked_Location_Should_Succeed()
    {
        LocationId firstLocationId = await CreateLocation("Главный офис");
        LocationId secondLocationId = await CreateLocation("Склад");
        DepartmentId departmentId = await CreateDepartment(firstLocationId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new LinkingLocationCommand(departmentId.Value, secondLocationId.Value), cancellationToken));

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var link = await dbContext.DepartmentLocations
                .FirstAsync(dl => dl.Id == result.Value, cancellationToken);

            Assert.Equal(departmentId, link.DepartmentId);
            Assert.Equal(secondLocationId, link.LocationId);
            Assert.Equal(2, await dbContext.DepartmentLocations.CountAsync(cancellationToken));
        });
    }

    [Fact]
    public async Task LinkingLocation_With_Already_Linked_Location_Should_Return_Conflict()
    {
        LocationId locationId = await CreateLocation("Главный офис");
        DepartmentId departmentId = await CreateDepartment(locationId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new LinkingLocationCommand(departmentId.Value, locationId.Value), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.CONFLICT, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            Assert.Equal(1, await dbContext.DepartmentLocations.CountAsync(cancellationToken));
        });
    }

    [Fact]
    public async Task LinkingLocation_With_Not_Existing_Location_Should_Return_NotFound()
    {
        LocationId locationId = await CreateLocation("Главный офис");
        DepartmentId departmentId = await CreateDepartment(locationId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new LinkingLocationCommand(departmentId.Value, Guid.NewGuid()), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NOT_FOUND, result.Error.Type);
    }

    [Fact]
    public async Task LinkingLocation_With_Empty_DepartmentId_Should_Return_Validation_Error()
    {
        LocationId locationId = await CreateLocation("Главный офис");

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new LinkingLocationCommand(Guid.Empty, locationId.Value), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.VALIDATION, result.Error.Type);
    }

    [Fact]
    public async Task UnlinkingLocation_With_Linked_Location_Should_Succeed()
    {
        LocationId firstLocationId = await CreateLocation("Главный офис");
        LocationId secondLocationId = await CreateLocation("Склад");
        DepartmentId departmentId = await CreateDepartment(firstLocationId, secondLocationId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new UnlinkingLocationCommand(departmentId.Value, secondLocationId.Value), cancellationToken));

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var link = await dbContext.DepartmentLocations.SingleAsync(cancellationToken);

            Assert.Equal(firstLocationId, link.LocationId);
            Assert.True(await dbContext.Locations.AnyAsync(l => l.Id == secondLocationId, cancellationToken));
        });
    }

    [Fact]
    public async Task UnlinkingLocation_With_Not_Linked_Location_Should_Return_NotFound()
    {
        LocationId firstLocationId = await CreateLocation("Главный офис");
        LocationId secondLocationId = await CreateLocation("Склад");
        DepartmentId departmentId = await CreateDepartment(firstLocationId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new UnlinkingLocationCommand(departmentId.Value, secondLocationId.Value), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NOT_FOUND, result.Error.Type);
    }

    [Fact]
    public async Task LinkingPosition_With_Not_Linked_Position_Should_Succeed()
    {
        LocationId locationId = await CreateLocation("Главный офис");
        DepartmentId departmentId = await CreateDepartment(locationId);
        PositionId positionId = await CreatePosition();

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new LinkingPositionCommand(departmentId.Value, positionId.Value), cancellationToken));

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var link = await dbContext.DepartmentPositions
                .FirstAsync(dp => dp.Id == result.Value, cancellationToken);

            Assert.Equal(departmentId, link.DepartmentId);
            Assert.Equal(positionId, link.PositionId);
        });
    }

    [Fact]
    public async Task LinkingPosition_With_Already_Linked_Position_Should_Return_Conflict()
    {
        LocationId locationId = await CreateLocation("Главный офис");
        DepartmentId departmentId = await CreateDepartment(locationId);
        PositionId positionId = await CreatePosition();
        await LinkPosition(departmentId, positionId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new LinkingPositionCommand(departmentId.Value, positionId.Value), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.CONFLICT, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            Assert.Equal(1, await dbContext.DepartmentPositions.CountAsync(cancellationToken));
        });
    }

    [Fact]
    public async Task LinkingPosition_With_Not_Existing_Position_Should_Return_NotFound()
    {
        LocationId locationId = await CreateLocation("Главный офис");
        DepartmentId departmentId = await CreateDepartment(locationId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new LinkingPositionCommand(departmentId.Value, Guid.NewGuid()), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NOT_FOUND, result.Error.Type);
    }

    [Fact]
    public async Task UnlinkingPosition_With_Linked_Position_Should_Succeed()
    {
        LocationId locationId = await CreateLocation("Главный офис");
        DepartmentId departmentId = await CreateDepartment(locationId);
        PositionId positionId = await CreatePosition();
        await LinkPosition(departmentId, positionId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new UnlinkingPositionCommand(departmentId.Value, positionId.Value), cancellationToken));

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            Assert.Equal(0, await dbContext.DepartmentPositions.CountAsync(cancellationToken));
            Assert.True(await dbContext.Positions.AnyAsync(p => p.Id == positionId, cancellationToken));
        });
    }

    [Fact]
    public async Task UnlinkingPosition_With_Not_Linked_Position_Should_Return_NotFound()
    {
        LocationId locationId = await CreateLocation("Главный офис");
        DepartmentId departmentId = await CreateDepartment(locationId);
        PositionId positionId = await CreatePosition();

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new UnlinkingPositionCommand(departmentId.Value, positionId.Value), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NOT_FOUND, result.Error.Type);
    }

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

    private async Task<DepartmentId> CreateDepartment(params LocationId[] locationIds)
    {
        return await ExecuteInDb(async dbContext =>
        {
            var departmentId = new DepartmentId(Guid.NewGuid());

            var departmentLocations = locationIds
                .Select(locationId => new DepartmentLocation
                {
                    Id = new DepartmentLocationId(Guid.NewGuid()),
                    LocationId = locationId,
                    DepartmentId = departmentId,
                    IsPrimary = false,
                })
                .ToList();

            var department = Domain.Departments.Department.CreateParent(
                DepartmentName.Create("Отдел продаж").Value,
                Identifier.Create("sales").Value,
                departmentLocations,
                departmentId).Value;

            dbContext.Add(department);

            await dbContext.SaveChangesAsync();

            return department.Id;
        });
    }

    private async Task<PositionId> CreatePosition()
    {
        return await ExecuteInDb(async dbContext =>
        {
            var position = new Domain.Positions.Position(
                PositionName.Create("Разработчик").Value,
                PositionDescription.Create("Пишет код").Value);

            dbContext.Add(position);

            await dbContext.SaveChangesAsync();

            return position.Id;
        });
    }

    private async Task LinkPosition(DepartmentId departmentId, PositionId positionId)
    {
        await ExecuteInDb(async dbContext =>
        {
            dbContext.Add(new DepartmentPosition
            {
                Id = new DepartmentPositionId(Guid.NewGuid()),
                DepartmentId = departmentId,
                PositionId = positionId,
            });

            await dbContext.SaveChangesAsync();
        });
    }
}
