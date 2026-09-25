using DirectoryService.Application.Departments.Commands.Create;
using DirectoryService.Contracts.Request.Department;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.IntegrationTests.Abstraction;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.EntityFrameworkCore;
using TimeZone = DirectoryService.Domain.Locations.ValueObjects.TimeZone;

namespace DirectoryService.IntegrationTests.Department;

public class CreateDepartmentTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    [Fact]
    public async Task CreateDepartment_With_Valid_Data_Should_Succeed()
    {
        LocationId locationId = await CreateLocation();

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                "Подразделение",
                "sales",
                null,
                [locationId.Value]));

            return sut.Send(command, cancellationToken);
        });

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == result.Value, cancellationToken);

            Assert.NotNull(department);
            Assert.Equal(department.Id, result.Value);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value.Value);
        });
    }

    [Fact]
    public async Task CreateDepartment_With_Parent_Should_Build_Path_And_Depth()
    {
        LocationId locationId = await CreateLocation();
        DepartmentId parentId = await CreateDepartment("Отдел продаж", "sales", locationId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                "B2B продажи",
                "b2b",
                parentId.Value,
                [locationId.Value]));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == result.Value, cancellationToken);

            Assert.Equal(parentId, department.ParentId);
            Assert.Equal("sales.b2b", department.Path.Value);
            Assert.Equal(1, department.Depth);
        });
    }

    [Fact]
    public async Task CreateDepartment_With_Not_Existing_Location_Should_Return_NotFound()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                "Подразделение",
                "sales",
                null,
                [Guid.NewGuid()]));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NOT_FOUND, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            Assert.Equal(0, await dbContext.Departments.CountAsync(cancellationToken));
        });
    }

    [Fact]
    public async Task CreateDepartment_With_Not_Existing_Parent_Should_Return_NotFound()
    {
        LocationId locationId = await CreateLocation();

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                "B2B продажи",
                "b2b",
                Guid.NewGuid(),
                [locationId.Value]));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NOT_FOUND, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            Assert.Equal(0, await dbContext.Departments.CountAsync(cancellationToken));
        });
    }

    [Fact]
    public async Task CreateDepartment_With_Invalid_Name_Should_Return_Validation_Error()
    {
        LocationId locationId = await CreateLocation();

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                "ab",
                "sales",
                null,
                [locationId.Value]));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.VALIDATION, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            Assert.Equal(0, await dbContext.Departments.CountAsync(cancellationToken));
        });
    }

    [Fact]
    public async Task CreateDepartment_With_Invalid_Slug_Should_Return_Validation_Error()
    {
        LocationId locationId = await CreateLocation();

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                "Подразделение",
                "ab",
                null,
                [locationId.Value]));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.VALIDATION, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            Assert.Equal(0, await dbContext.Departments.CountAsync(cancellationToken));
        });
    }

    private async Task<LocationId> CreateLocation()
    {
        return await ExecuteInDb(async dbContext =>
        {
            var location = new Domain.Locations.Location(
                LocationName.Create("Локация").Value,
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
