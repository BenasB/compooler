using Compooler.Application.Commands;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Application.Tests.Commands;

[Collection(ApplicationTestsCollection.Name)]
public class CommuteGroupCommandsTests(ApplicationFixture fixture) : IAsyncLifetime
{
    private readonly CompoolerDbContext _dbContext = new(fixture.DbContextOptions);

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _dbContext.DisposeAsync().AsTask();

    private async Task<int> CreateDriver()
    {
        var firstName = Guid.NewGuid().ToString("N");
        const string lastName = "Driver";
        var driver = _dbContext.Users.Add(User.Create(firstName, lastName));
        await _dbContext.SaveChangesAsync();
        return driver.Entity.Id;
    }

    [Fact]
    public async Task CreateCommuteGroup_Succeeds()
    {
        var driverId = await CreateDriver();
        var command = new CreateCommuteGroupCommand(
            DriverId: driverId,
            MaxPassengers: 3,
            StartLatitude: 0,
            StartLongitude: 0,
            FinishLatitude: 0,
            FinishLongitude: 0
        );
        var handler = new CreateCommuteGroupHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsFailed);
        Assert.NotNull(
            await _dbContext.CommuteGroups.FirstOrDefaultAsync(x => x.Id == result.Value.Id)
        );
    }

    [Fact]
    public async Task CreateCommuteGroup_DriverDoesNotExist_Fails()
    {
        const int nonExistentId = -1;
        var command = new CreateCommuteGroupCommand(
            DriverId: nonExistentId,
            MaxPassengers: 3,
            StartLatitude: 0,
            StartLongitude: 0,
            FinishLatitude: 0,
            FinishLongitude: 0
        );

        var handler = new CreateCommuteGroupHandler(_dbContext);
        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Equal(new EntityNotFoundError<User>(Id: nonExistentId), result.Error);
    }

    [Fact]
    public async Task CreateCommuteGroup_InvalidData_Fails()
    {
        var driverId = await CreateDriver();

        var command = new CreateCommuteGroupCommand(
            DriverId: driverId,
            MaxPassengers: 3,
            StartLatitude: -91,
            StartLongitude: 181,
            FinishLatitude: 91,
            FinishLongitude: -181
        );
        var handler = new CreateCommuteGroupHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
    }
}
