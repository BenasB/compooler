using Compooler.Application.Commands;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence;
using Compooler.Persistence.Configurations;
using Compooler.Tests.Utilities;
using Compooler.Tests.Utilities.Fixtures;
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

    private async Task PersistNewRidePassenger(params (int rideId, string userId)[] ridePassengers)
    {
        foreach (var (rideId, userId) in ridePassengers)
        {
            var entity = _dbContext.RidePassengers.Add(RidePassenger.Create(userId));
            entity.Property<int>(RideConfiguration.RideIdColumnName).CurrentValue = rideId;
        }

        await _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task CreateRide_Succeeds()
    {
        var dateTimeOffsetProvider = new FixedDateTimeOffsetProvider
        {
            Now = DateTimeOffset.Now.ToUniversalTime()
        };
        var driver = await PersistNewUser();

        var command = new CreateRideCommand(
            DriverId: driver.Id,
            MaxPassengers: 3,
            StartLatitude: 0,
            StartLongitude: 0,
            FinishLatitude: 0,
            FinishLongitude: 0,
            TimeOfDeparture: dateTimeOffsetProvider.Future
        );
        var handler = new CreateRideCommandHandler(_dbContext, dateTimeOffsetProvider);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsFailed);
        Assert.NotNull(await _dbContext.Rides.FirstOrDefaultAsync(x => x.Id == result.Value.Id));
    }

    [Fact]
    public async Task CreateRide_DriverDoesNotExist_Fails()
    {
        var dateTimeOffsetProvider = new FixedDateTimeOffsetProvider
        {
            Now = DateTimeOffset.Now.ToUniversalTime()
        };
        var nonExistentUserId = TestEntityFactory.CreateUserId();
        var command = new CreateRideCommand(
            DriverId: nonExistentUserId,
            MaxPassengers: 3,
            StartLatitude: 0,
            StartLongitude: 0,
            FinishLatitude: 0,
            FinishLongitude: 0,
            TimeOfDeparture: dateTimeOffsetProvider.Future
        );

        var handler = new CreateRideCommandHandler(_dbContext, dateTimeOffsetProvider);
        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Equal(new EntityNotFoundError<User, string>(Id: nonExistentUserId), result.Error);
    }

    [Fact]
    public async Task CreateRide_InvalidData_Fails()
    {
        var dateTimeOffsetProvider = new FixedDateTimeOffsetProvider
        {
            Now = DateTimeOffset.Now.ToUniversalTime()
        };
        var driver = await PersistNewUser();

        var command = new CreateRideCommand(
            DriverId: driver.Id,
            MaxPassengers: 3,
            StartLatitude: -91,
            StartLongitude: 181,
            FinishLatitude: 91,
            FinishLongitude: -181,
            TimeOfDeparture: dateTimeOffsetProvider.Future
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
        await PersistNewRidePassenger((ride.Id, passenger.Id));

        var newRideId = ride.Id;
        var command = new RemoveRideCommand(Id: newRideId, UserId: ride.DriverId);
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
        var command = new RemoveRideCommand(
            Id: nonExistentId,
            UserId: TestEntityFactory.CreateUserId()
        );
        var handler = new RemoveRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Equal(new EntityNotFoundError<Ride, int>(nonExistentId), result.Error);
    }

    [Fact]
    public async Task RemoveRide_UserIsNotDriver_Fails()
    {
        var ride = await PersistNewRide();

        var newRideId = ride.Id;
        var command = new RemoveRideCommand(
            Id: newRideId,
            UserId: TestEntityFactory.CreateUserId()
        );
        var handler = new RemoveRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Equal(new RideErrors.UserIsNotDriverError(), result.Error);
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
        Assert.Equal(new EntityNotFoundError<Ride, int>(nonExistentRideId), result.Error);
    }

    [Fact]
    public async Task JoinRide_UserDoesNotExist_Fails()
    {
        var ride = await PersistNewRide();
        var nonExistentUserId = TestEntityFactory.CreateUserId();

        var command = new JoinRideCommand(RideId: ride.Id, UserId: nonExistentUserId);
        var handler = new JoinRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Equal(new EntityNotFoundError<User, string>(nonExistentUserId), result.Error);
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
        Assert.Equal(new EntityNotFoundError<Ride, int>(nonExistentRideId), result.Error);
    }

    [Fact]
    public async Task LeaveRide_UserDoesNotExist_Fails()
    {
        var ride = await PersistNewRide();
        var nonExistentUserId = TestEntityFactory.CreateUserId();

        var command = new LeaveRideCommand(RideId: ride.Id, UserId: nonExistentUserId);
        var handler = new LeaveRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Equal(new EntityNotFoundError<User, string>(nonExistentUserId), result.Error);
    }

    [Fact]
    public async Task LeaveRide_UserAndRideExist_Succeeds()
    {
        var user = await PersistNewUser();
        var ride = await PersistNewRide();
        await PersistNewRidePassenger((ride.Id, user.Id));

        var command = new LeaveRideCommand(RideId: ride.Id, UserId: user.Id);
        var handler = new LeaveRideCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsFailed);
        ride = await _dbContext.Rides.FirstAsync(x => x.Id == ride.Id);
        Assert.Empty(ride.Passengers);
    }
}
