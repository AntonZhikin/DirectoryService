using DirectoryService.Application.Departments.Commands.Create;
using DirectoryService.Contracts.Request.Department;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TimeZone = DirectoryService.Domain.Locations.ValueObjects.TimeZone;

namespace DirectoryService.IntegrationTests;

public class CreateDepartmentTests : DirectoryTestWebFactory
{
    [Fact]
    public async Task CreateDepartment_With_Valid_Data_Should_Succeed()
    {
        // arrange   
        LocationId locationId = await CreateLocation();

        await using var scope = Services.CreateAsyncScope(); 
        
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        var cancellationToken = CancellationToken.None;

        var command = new CreateDepartmentCommand(new CreateDepartmentRequest(

            "Подразделение",
            "sales",
            null,
            [locationId.Value]));

        // act
        var result = await mediator.Send(command, cancellationToken);
        
        // assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Value);
    }

    private async Task<LocationId> CreateLocation()
    {
        await using var initializerScope = Services.CreateAsyncScope();
        var dbContext = initializerScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
        var location = new Location(
            LocationName.Create("Локация").Value,
            TimeZone.Create("Europe/Moscow").Value,
            new Address("москва","пушкина", "12", "10"));

        dbContext.Add(location);
            
        await dbContext.SaveChangesAsync();

        return location.Id;
    }
}