using Compooler.Application.Commands;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Domain.Tests.Utilities;
using Compooler.Persistence;
using Compooler.Persistence.Configurations;
using Compooler.Persistence.Tests;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Application.Tests.Commands;

[Collection(ApplicationTestsCollection.Name)]
public class RideCommandsTests(DatabaseFixture fixture) : IAsyncLifetime
{
    private readonly CompoolerDbContext _dbContext = new(fixture.DbContextOptions);

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _dbContext.DisposeAsync().AsTask();

    private async Task<User> PersistNewUser()
    {
        var user = TestEntityFactory.CreateUser();
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    private async Task<Ride> PersistNewRide()
    {
        var ride = TestEntityFactory.CreateRide().RequiredSuccess();
        _dbContext.Rides.Add(ride);
        await _dbContext.SaveChangesAsync();

        return ride;
    }

    [Fact]
    public async Task CreateRide_Succeeds()
    {
        var dateTimeOffsetProvider = new FixedDateTimeOffsetProvider { Now = DateTimeOffset.Now };
        var driver = await PersistNewUser();

        var command = new CreateRideCommand(
            DriverId: driver.Id,
            MaxPassengers: 3,
            StartLatitude: 0,
            StartLongitude: 0,
            FinishLatitude: 0,
            FinishLongitude: 0,
            TimeOfDeparture: dateTimeOffsetProvider.Future.ToUniversalTime()
        );
        var handler = new CreateRideCommandHandler(_dbContext, dateTimeOffsetProvider);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsFailed);
        Assert.NotNull(await _dbContext.Rides.FirstOrDefaultAsync(x => x.Id == result.Value.Id));
    }

    [Fact]
    public async Task CreateRide_DriverDoesNotExist_Fails()
    {
        var dateTimeOffsetProvider = new FixedDateTimeOffsetProvider { Now = DateTimeOffset.Now };
        const int nonExistentId = -1;
        var command = new CreateRideCommand(
            DriverId: nonExistentId,
            MaxPassengers: 3,
            StartLatitude: 0,
            StartLongitude: 0,
            FinishLatitude: 0,
            FinishLongitude: 0,
            TimeOfDeparture: dateTimeOffsetProvider.Future.ToUniversalTime()
        );

        var handler = new CreateRideCommandHandler(_dbContext, dateTimeOffsetProvider);
        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Equal(new EntityNotFoundError<User>(Id: nonExistentId), result.Error);
    }

    [Fact]
    public async Task CreateRide_InvalidData_Fails()
    {
        var dateTimeOffsetProvider = new FixedDateTimeOffsetProvider { Now = DateTimeOffset.Now };
        var driver = await PersistNewUser();

        var command = new CreateRideCommand(
            DriverId: driver.Id,
            MaxPassengers: 3,
            StartLatitude: -91,
            StartLongitude: 181,
            FinishLatitude: 91,
            FinishLongitude: -181,
            TimeOfDeparture: dateTimeOffsetProvider.Future.ToUniversalTime()
        );
        var handler = new CreateRideCommandHandler(_dbContext, dateTimeOffsetProvider);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task RemoveRide_RideExists_Succeeds()
    {
        var passenger = await PersistNewUser();
        var ride = await PersistNewRide();

        Assert.False(
            ride.AddPassenger(passenger.Id).IsFailed,
            "Failed to add passenger which is a prerequisite"
        );
        await _dbContext.SaveChangesAsync();

        var newRideId = ride.Id;
        var command = new RemoveRideCommand(Id: newRideId);
        var handler = new RemoveRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsFailed);
        Assert.Null(await _dbContext.Rides.FirstOrDefaultAsync(x => x.Id == ride.Id));
        Assert.Empty(
            await _dbContext
                .RidePassengers.Where(x =>
                    EF.Property<int>(x, RideConfiguration.RideIdColumnName) == newRideId
                )
                .ToListAsync()
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

    [Fact]
    public async Task JoinRide_RideDoesNotExist_Fails()
    {
        var user = await PersistNewUser();
        const int nonExistentRideId = -1;

        var command = new JoinRideCommand(RideId: nonExistentRideId, UserId: user.Id);
        var handler = new JoinRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Equal(new EntityNotFoundError<Ride>(nonExistentRideId), result.Error);
    }

    [Fact]
    public async Task JoinRide_UserDoesNotExist_Fails()
    {
        var ride = await PersistNewRide();
        const int nonExistentUserId = -1;

        var command = new JoinRideCommand(RideId: ride.Id, UserId: nonExistentUserId);
        var handler = new JoinRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Equal(new EntityNotFoundError<User>(nonExistentUserId), result.Error);
    }

    [Fact]
    public async Task JoinRide_UserAndRideExist_Succeeds()
    {
        var user = await PersistNewUser();
        var ride = await PersistNewRide();

        var command = new JoinRideCommand(RideId: ride.Id, UserId: user.Id);
        var handler = new JoinRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsFailed);
        ride = await _dbContext.Rides.FirstAsync(x => x.Id == ride.Id);
        Assert.Single(ride.Passengers);
    }

    [Fact]
    public async Task LeaveRide_RideDoesNotExist_Fails()
    {
        var user = await PersistNewUser();
        const int nonExistentRideId = -1;

        var command = new LeaveRideCommand(RideId: nonExistentRideId, UserId: user.Id);
        var handler = new LeaveRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Equal(new EntityNotFoundError<Ride>(nonExistentRideId), result.Error);
    }

    [Fact]
    public async Task LeaveRide_UserDoesNotExist_Fails()
    {
        var ride = await PersistNewRide();
        const int nonExistentUserId = -1;

        var command = new LeaveRideCommand(RideId: ride.Id, UserId: nonExistentUserId);
        var handler = new LeaveRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Equal(new EntityNotFoundError<User>(nonExistentUserId), result.Error);
    }

    [Fact]
    public async Task LeaveRide_UserAndRideExist_Succeeds()
    {
        var user = await PersistNewUser();
        var ride = await PersistNewRide();

        Assert.False(
            ride.AddPassenger(user.Id).IsFailed,
            "Failed to add passenger which is a prerequisite"
        );
        await _dbContext.SaveChangesAsync();

        var command = new LeaveRideCommand(RideId: ride.Id, UserId: user.Id);
        var handler = new LeaveRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsFailed);
        ride = await _dbContext.Rides.FirstAsync(x => x.Id == ride.Id);
        Assert.Empty(ride.Passengers);
    }
}
