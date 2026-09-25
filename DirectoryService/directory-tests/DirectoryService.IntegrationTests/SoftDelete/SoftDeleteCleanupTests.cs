using DirectoryService.Application.Departments.Commands.Delete;
using DirectoryService.Application.Locations.Commands.Delete;
using DirectoryService.Application.Positions.Delete;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.Infrastructure.BackgroundServices;
using DirectoryService.Infrastructure.Database;
using DirectoryService.IntegrationTests.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TimeZone = DirectoryService.Domain.Locations.ValueObjects.TimeZone;

namespace DirectoryService.IntegrationTests.SoftDelete;

public class SoftDeleteCleanupTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    [Fact]
    public async Task Cleanup_Should_Remove_Records_Deleted_Before_Threshold()
    {
        LocationId deletedLocationId = await CreateLocation("Старый офис");
        LocationId activeLocationId = await CreateLocation("Главный офис");
        DepartmentId departmentId = await CreateDepartment("Отдел продаж", "sales", activeLocationId);
        PositionId positionId = await CreatePosition();

        var cancellationToken = CancellationToken.None;

        await ExecuteHandler((sut) =>
            sut.Send(new DeleteLocationCommand(deletedLocationId.Value), cancellationToken));
        await ExecuteHandler((sut) =>
            sut.Send(new DeleteDepartmentCommand(departmentId.Value), cancellationToken));
        await ExecuteHandler((sut) =>
            sut.Send(new DeletePositionCommand(positionId.Value), cancellationToken));

        var cleaner = Services.GetRequiredService<SoftDeleteCleaner>();

        int deleted = await cleaner.CleanupAsync(DateTime.UtcNow.AddMinutes(1), 1, cancellationToken);

        Assert.Equal(3, deleted);

        await ExecuteInDb(async dbContext =>
        {
            Assert.False(await dbContext.Locations.IgnoreQueryFilters()
                .AnyAsync(l => l.Id == deletedLocationId, cancellationToken));
            Assert.False(await dbContext.Departments.IgnoreQueryFilters()
                .AnyAsync(d => d.Id == departmentId, cancellationToken));
            Assert.False(await dbContext.Positions.IgnoreQueryFilters()
                .AnyAsync(p => p.Id == positionId, cancellationToken));
            Assert.Equal(0, await dbContext.DepartmentLocations.CountAsync(cancellationToken));
            Assert.True(await dbContext.Locations.AnyAsync(l => l.Id == activeLocationId, cancellationToken));
        });
    }

    [Fact]
    public async Task Cleanup_Should_Keep_Records_Deleted_After_Threshold()
    {
        LocationId locationId = await CreateLocation("Главный офис");

        var cancellationToken = CancellationToken.None;

        await ExecuteHandler((sut) =>
            sut.Send(new DeleteLocationCommand(locationId.Value), cancellationToken));

        var cleaner = Services.GetRequiredService<SoftDeleteCleaner>();

        int deleted = await cleaner.CleanupAsync(DateTime.UtcNow.AddDays(-1), 100, cancellationToken);

        Assert.Equal(0, deleted);

        await ExecuteInDb(async dbContext =>
        {
            var location = await dbContext.Locations
                .IgnoreQueryFilters()
                .FirstAsync(l => l.Id == locationId, cancellationToken);

            Assert.True(location.IsDeleted);
        });
    }

    [Fact]
    public async Task Cleanup_Should_Remove_Deleted_Child_And_Parent_Departments()
    {
        LocationId locationId = await CreateLocation("Главный офис");
        DepartmentId parentId = await CreateDepartment("Отдел продаж", "sales", locationId);
        DepartmentId childId = await CreateDepartment("B2B продажи", "b2b", locationId, parentId);

        var cancellationToken = CancellationToken.None;

        await ExecuteHandler((sut) =>
            sut.Send(new DeleteDepartmentCommand(childId.Value), cancellationToken));
        await ExecuteHandler((sut) =>
            sut.Send(new DeleteDepartmentCommand(parentId.Value), cancellationToken));

        var cleaner = Services.GetRequiredService<SoftDeleteCleaner>();

        int deleted = await cleaner.CleanupAsync(DateTime.UtcNow.AddMinutes(1), 1, cancellationToken);

        Assert.Equal(2, deleted);

        await ExecuteInDb(async dbContext =>
        {
            Assert.Equal(0, await dbContext.Departments.IgnoreQueryFilters().CountAsync(cancellationToken));
            Assert.Equal(0, await dbContext.DepartmentLocations.CountAsync(cancellationToken));
            Assert.True(await dbContext.Locations.AnyAsync(l => l.Id == locationId, cancellationToken));
        });
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
}
