using DirectoryService.Application.Departments.Commands.Delete;
using DirectoryService.Application.Departments.Commands.Update;
using DirectoryService.Application.Departments.Queries.GetById;
using DirectoryService.Application.Departments.Queries.GetList;
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

public class DepartmentTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    [Fact]
    public async Task UpdateDepartment_With_Valid_Name_Should_Succeed()
    {
        LocationId locationId = await CreateLocation();
        DepartmentId departmentId = await CreateDepartment("Отдел продаж", "sales", locationId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new UpdateDepartmentCommand(
                departmentId.Value,
                new UpdateDepartmentRequest("Коммерческий отдел"));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == departmentId, cancellationToken);

            Assert.Equal("Коммерческий отдел", department.Name.Value);
        });
    }

    [Fact]
    public async Task UpdateDepartment_With_Invalid_Name_Should_Return_Validation_Error()
    {
        LocationId locationId = await CreateLocation();
        DepartmentId departmentId = await CreateDepartment("Отдел продаж", "sales", locationId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new UpdateDepartmentCommand(
                departmentId.Value,
                new UpdateDepartmentRequest("ab"));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.VALIDATION, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == departmentId, cancellationToken);

            Assert.Equal("Отдел продаж", department.Name.Value);
        });
    }

    [Fact]
    public async Task UpdateDepartment_With_Not_Existing_Id_Should_Return_NotFound()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new UpdateDepartmentCommand(
                Guid.NewGuid(),
                new UpdateDepartmentRequest("Коммерческий отдел"));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NOT_FOUND, result.Error.Type);
    }

    [Fact]
    public async Task DeleteDepartment_Without_Children_Should_Succeed()
    {
        LocationId locationId = await CreateLocation();
        DepartmentId departmentId = await CreateDepartment("Отдел продаж", "sales", locationId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new DeleteDepartmentCommand(departmentId.Value), cancellationToken));

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            Assert.False(await dbContext.Departments.AnyAsync(d => d.Id == departmentId, cancellationToken));
            Assert.Equal(0, await dbContext.DepartmentLocations.CountAsync(cancellationToken));
            Assert.True(await dbContext.Locations.AnyAsync(l => l.Id == locationId, cancellationToken));
        });
    }

    [Fact]
    public async Task DeleteDepartment_With_Children_Should_Return_Conflict()
    {
        LocationId locationId = await CreateLocation();
        DepartmentId parentId = await CreateDepartment("Отдел продаж", "sales", locationId);
        await CreateDepartment("B2B продажи", "b2b", locationId, parentId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new DeleteDepartmentCommand(parentId.Value), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.CONFLICT, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            Assert.Equal(2, await dbContext.Departments.CountAsync(cancellationToken));
        });
    }

    [Fact]
    public async Task DeleteDepartment_With_Not_Existing_Id_Should_Return_NotFound()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new DeleteDepartmentCommand(Guid.NewGuid()), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NOT_FOUND, result.Error.Type);
    }

    [Fact]
    public async Task GetDepartmentById_With_Existing_Id_Should_Return_Department()
    {
        LocationId locationId = await CreateLocation();
        DepartmentId parentId = await CreateDepartment("Отдел продаж", "sales", locationId);
        DepartmentId childId = await CreateDepartment("B2B продажи", "b2b", locationId, parentId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new GetDepartmentByIdQuery(childId.Value), cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(childId.Value, result.Value.Id);
        Assert.Equal(parentId.Value, result.Value.ParentId);
        Assert.Equal("B2B продажи", result.Value.Name);
        Assert.Equal("sales.b2b", result.Value.Path);
        Assert.Equal(1, result.Value.Depth);
    }

    [Fact]
    public async Task GetDepartmentById_With_Not_Existing_Id_Should_Return_NotFound()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new GetDepartmentByIdQuery(Guid.NewGuid()), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NOT_FOUND, result.Error.Type);
    }

    [Fact]
    public async Task GetDepartments_With_Paging_Should_Return_Page_Sorted_By_Name()
    {
        LocationId locationId = await CreateLocation();
        await CreateDepartment("Backend", "backend", locationId);
        await CreateDepartment("Analytics", "analytics", locationId);
        await CreateDepartment("Design", "design", locationId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new GetDepartmentsQuery(new GetDepartmentsRequest(null, null, null, 2, 1)), cancellationToken));

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.TotalCount);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.Equal("Analytics", result.Value.Items[0].Name);
        Assert.Equal("Backend", result.Value.Items[1].Name);
    }

    [Fact]
    public async Task GetDepartments_With_Search_Should_Return_Only_Matching()
    {
        LocationId locationId = await CreateLocation();
        await CreateDepartment("Backend", "backend", locationId);
        await CreateDepartment("Analytics", "analytics", locationId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new GetDepartmentsQuery(new GetDepartmentsRequest("end", null, null)), cancellationToken));

        Assert.True(result.IsSuccess);

        var department = Assert.Single(result.Value.Items);
        Assert.Equal("Backend", department.Name);
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

    private async Task<DepartmentId> CreateDepartment(
        string name,
        string slug,
        LocationId locationId,
        DepartmentId? parentId = null)
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

            var departmentName = DepartmentName.Create(name).Value;
            var identifier = Identifier.Create(slug).Value;

            Domain.Departments.Department department;
            if (parentId is null)
            {
                department = Domain.Departments.Department.CreateParent(departmentName, identifier, departmentLocations, departmentId).Value;
            }
            else
            {
                var parent = await dbContext.Departments.FirstAsync(d => d.Id == parentId);
                department = Domain.Departments.Department.CreateChild(departmentName, identifier, parent, departmentLocations, departmentId).Value;
            }

            dbContext.Add(department);

            await dbContext.SaveChangesAsync();

            return department.Id;
        });
    }
}
