using DirectoryService.Application.Positions.Create;
using DirectoryService.Application.Positions.Delete;
using DirectoryService.Application.Positions.Update;
using DirectoryService.Contracts.Request.Position;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.IntegrationTests.Abstraction;
using DirectoryService.Shared.ErrorManagement;
using Microsoft.EntityFrameworkCore;
using TimeZone = DirectoryService.Domain.Locations.ValueObjects.TimeZone;

namespace DirectoryService.IntegrationTests.Position;

public class PositionTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    [Fact]
    public async Task CreatePosition_With_Valid_Data_Should_Succeed()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new CreatePositionCommand(new CreatePositionRequest("Разработчик", "Пишет код")), cancellationToken));

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var position = await dbContext.Positions
                .FirstAsync(p => p.Id == result.Value, cancellationToken);

            Assert.Equal("Разработчик", position.Name.Value);
            Assert.Equal("Пишет код", position.Description.Value);
            Assert.True(position.IsActive);
        });
    }

    [Fact]
    public async Task CreatePosition_With_Empty_Name_Should_Return_Validation_Error()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new CreatePositionCommand(new CreatePositionRequest("", "Пишет код")), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.VALIDATION, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            Assert.Equal(0, await dbContext.Positions.CountAsync(cancellationToken));
        });
    }

    [Fact]
    public async Task CreatePosition_With_Too_Long_Description_Should_Return_Validation_Error()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new CreatePositionCommand(new CreatePositionRequest("Разработчик", new string('a', 101))), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.VALIDATION, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            Assert.Equal(0, await dbContext.Positions.CountAsync(cancellationToken));
        });
    }

    [Fact]
    public async Task UpdatePosition_With_Valid_Data_Should_Succeed()
    {
        PositionId positionId = await CreatePosition();

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new UpdatePositionCommand(
                positionId.Value,
                new UpdatePositionRequest("Тимлид", "Руководит командой"));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var position = await dbContext.Positions
                .FirstAsync(p => p.Id == positionId, cancellationToken);

            Assert.Equal("Тимлид", position.Name.Value);
            Assert.Equal("Руководит командой", position.Description.Value);
        });
    }

    [Fact]
    public async Task UpdatePosition_With_Empty_Name_Should_Return_Validation_Error()
    {
        PositionId positionId = await CreatePosition();

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new UpdatePositionCommand(
                positionId.Value,
                new UpdatePositionRequest("", "Руководит командой"));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.VALIDATION, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            var position = await dbContext.Positions
                .FirstAsync(p => p.Id == positionId, cancellationToken);

            Assert.Equal("Разработчик", position.Name.Value);
        });
    }

    [Fact]
    public async Task UpdatePosition_With_Not_Existing_Id_Should_Return_NotFound()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
        {
            var command = new UpdatePositionCommand(
                Guid.NewGuid(),
                new UpdatePositionRequest("Тимлид", "Руководит командой"));

            return sut.Send(command, cancellationToken);
        });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NOT_FOUND, result.Error.Type);
    }

    [Fact]
    public async Task DeletePosition_Without_Departments_Should_Succeed()
    {
        PositionId positionId = await CreatePosition();

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new DeletePositionCommand(positionId.Value), cancellationToken));

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            Assert.False(await dbContext.Positions.AnyAsync(p => p.Id == positionId, cancellationToken));
        });
    }

    [Fact]
    public async Task DeletePosition_With_Linked_Department_Should_Return_Conflict()
    {
        PositionId positionId = await CreatePosition();
        await LinkPositionToNewDepartment(positionId);

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new DeletePositionCommand(positionId.Value), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.CONFLICT, result.Error.Type);

        await ExecuteInDb(async dbContext =>
        {
            Assert.True(await dbContext.Positions.AnyAsync(p => p.Id == positionId, cancellationToken));
        });
    }

    [Fact]
    public async Task DeletePosition_With_Not_Existing_Id_Should_Return_NotFound()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler((sut) =>
            sut.Send(new DeletePositionCommand(Guid.NewGuid()), cancellationToken));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NOT_FOUND, result.Error.Type);
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

    private async Task LinkPositionToNewDepartment(PositionId positionId)
    {
        await ExecuteInDb(async dbContext =>
        {
            var location = new Domain.Locations.Location(
                LocationName.Create("Локация").Value,
                TimeZone.Create("Europe/Moscow").Value,
                new Address("москва", "пушкина", "12", "10"));

            var departmentId = new DepartmentId(Guid.NewGuid());

            List<DepartmentLocation> departmentLocations =
            [
                new()
                {
                    Id = new DepartmentLocationId(Guid.NewGuid()),
                    LocationId = location.Id,
                    DepartmentId = departmentId,
                    IsPrimary = false,
                },
            ];

            var department = Domain.Departments.Department.CreateParent(
                DepartmentName.Create("Отдел продаж").Value,
                Identifier.Create("sales").Value,
                departmentLocations,
                departmentId).Value;

            dbContext.Add(location);
            dbContext.Add(department);
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
