using Compooler.Application.Commands;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence;
using Compooler.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Application.Tests.Commands;

[Collection(ApplicationTestsCollection.Name)]
public class RideCommandsTests(ApplicationFixture fixture) : IAsyncLifetime
{
    private readonly CompoolerDbContext _dbContext = new(fixture.DbContextOptions);

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _dbContext.DisposeAsync().AsTask();

    private async Task<int> CreateUser()
    {
        var firstName = Guid.NewGuid().ToString("N");
        const string lastName = "Driver";
        var driver = _dbContext.Users.Add(User.Create(firstName, lastName));
        await _dbContext.SaveChangesAsync();
        return driver.Entity.Id;
    }

    [Fact]
    public async Task CreateRide_Succeeds()
    {
        var driverId = await CreateUser();
        var command = new CreateRideCommand(
            DriverId: driverId,
            MaxPassengers: 3,
            StartLatitude: 0,
            StartLongitude: 0,
            FinishLatitude: 0,
            FinishLongitude: 0
        );
        var handler = new CreateRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsFailed);
        Assert.NotNull(await _dbContext.Rides.FirstOrDefaultAsync(x => x.Id == result.Value.Id));
    }

    [Fact]
    public async Task CreateRide_DriverDoesNotExist_Fails()
    {
        const int nonExistentId = -1;
        var command = new CreateRideCommand(
            DriverId: nonExistentId,
            MaxPassengers: 3,
            StartLatitude: 0,
            StartLongitude: 0,
            FinishLatitude: 0,
            FinishLongitude: 0
        );

        var handler = new CreateRideCommandHandler(_dbContext);
        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Equal(new EntityNotFoundError<User>(Id: nonExistentId), result.Error);
    }

    [Fact]
    public async Task CreateRide_InvalidData_Fails()
    {
        var driverId = await CreateUser();

        var command = new CreateRideCommand(
            DriverId: driverId,
            MaxPassengers: 3,
            StartLatitude: -91,
            StartLongitude: 181,
            FinishLatitude: 91,
            FinishLongitude: -181
        );
        var handler = new CreateRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task RemoveRide_RideExists_Succeeds()
    {
        var driverId = await CreateUser();
        var passengerId = await CreateUser();
        var startCoordsResult = GeographicCoordinates.Create(0, 0);
        var finishCoordsResult = GeographicCoordinates.Create(0, 0);
        if (startCoordsResult.IsFailed || finishCoordsResult.IsFailed)
            throw new InvalidOperationException("Expected a successful creation of coordinates");

        var newRide = _dbContext.Rides.Add(
            Ride.Create(
                Route.Create(startCoordsResult.Value, finishCoordsResult.Value),
                driverId,
                1
            )
        );
        newRide.Entity.AddPassenger(passengerId);
        await _dbContext.SaveChangesAsync();

        var newRideId = newRide.Entity.Id;
        var command = new RemoveRideCommand(Id: newRideId);
        var handler = new RemoveRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsFailed);
        Assert.Null(await _dbContext.Rides.FirstOrDefaultAsync(x => x.Id == newRide.Entity.Id));
        Assert.Null(
            await _dbContext.RidePassengers.FirstOrDefaultAsync(x =>
                EF.Property<int>(x, RideConfiguration.RideIdColumnName) == newRideId
            )
        );
    }

    [Fact]
    public async Task RemoveRide_RideDoesNotExist_Fails()
    {
        const int nonExistentId = -1;
        var command = new RemoveRideCommand(Id: nonExistentId);
        var handler = new RemoveRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Equal(new EntityNotFoundError<Ride>(nonExistentId), result.Error);
    }
}
