using Compooler.Domain.Entities.RideEntity;
using Compooler.Persistence.Configurations;
using Compooler.Persistence.DataLoaders;
using Compooler.Tests.Utilities;
using Compooler.Tests.Utilities.Fixtures;
using HotChocolate.Pagination;

namespace Compooler.Persistence.Tests.DataLoaders;

[Collection(PersistenceTestsCollection.Name)]
public class RideIdsByUserId(DatabaseFixture fixture) : IAsyncLifetime
{
    private readonly CompoolerDbContext _dbContext = new(fixture.DbContextOptions);

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _dbContext.DisposeAsync().AsTask();

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
    private async Task UserIsOnlyDriver_ReturnsRides()
    {
        var user = TestEntityFactory.CreateUser();
        _dbContext.Users.Add(user);

        var rides = Enumerable
            .Range(0, 3)
            .Select(_ => TestEntityFactory.CreateRide(driverId: user.Id).RequiredSuccess())
            .ToList();

        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();

        var result = await RideDataLoaders.GetRideIdsByUserIdAsync(
            [user.Id],
            _dbContext,
            new PagingArguments(first: rides.Count + 1),
            cancellationToken: default
        );

        Assert.Equivalent(
            rides.Select(r => r.Id),
            result[user.Id].Items.Select(row => row.RideId),
            strict: true
        );
    }

    [Fact]
    private async Task UserIsOnlyPassenger_ReturnsRides()
    {
        var driver = TestEntityFactory.CreateUser();
        var user = TestEntityFactory.CreateUser();
        _dbContext.Users.AddRange([driver, user]);
        await _dbContext.SaveChangesAsync();

        var rides = Enumerable
            .Range(0, 3)
            .Select(_ => TestEntityFactory.CreateRide(driverId: driver.Id).RequiredSuccess())
            .ToList();

        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();

        await PersistNewRidePassenger(rides.Select(r => (r.Id, user.Id)).ToArray());

        var result = await RideDataLoaders.GetRideIdsByUserIdAsync(
            [user.Id],
            _dbContext,
            new PagingArguments(first: rides.Count + 1),
            cancellationToken: default
        );

        Assert.Equivalent(
            rides.Select(r => r.Id),
            result[user.Id].Items.Select(row => row.RideId),
            strict: true
        );
    }

    [Fact]
    private async Task UserIsDriverOrPassenger_ReturnsRides()
    {
        var user1 = TestEntityFactory.CreateUser();
        var user2 = TestEntityFactory.CreateUser();
        _dbContext.Users.AddRange([user1, user2]);
        await _dbContext.SaveChangesAsync();

        var drivingRides = Enumerable
            .Range(0, 1)
            .Select(_ => TestEntityFactory.CreateRide(driverId: user1.Id).RequiredSuccess())
            .ToList();

        var passengerRides = Enumerable
            .Range(0, 2)
            .Select(_ => TestEntityFactory.CreateRide(driverId: user2.Id).RequiredSuccess())
            .ToList();

        var rides = drivingRides.Concat(passengerRides).ToList();

        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();

        await PersistNewRidePassenger(passengerRides.Select(r => (r.Id, user1.Id)).ToArray());

        var result = await RideDataLoaders.GetRideIdsByUserIdAsync(
            [user1.Id],
            _dbContext,
            new PagingArguments(first: rides.Count + 1),
            cancellationToken: default
        );

        Assert.Equivalent(
            rides.Select(r => r.Id),
            result[user1.Id].Items.Select(row => row.RideId),
            strict: true
        );
    }

    [Fact]
    private async Task UserIsNeitherDriverNorPassenger_ReturnsNothing()
    {
        var user1 = TestEntityFactory.CreateUser();
        var user2 = TestEntityFactory.CreateUser();
        var user3 = TestEntityFactory.CreateUser();
        _dbContext.Users.AddRange([user1, user2, user3]);
        await _dbContext.SaveChangesAsync();

        var user1DrivingRides = Enumerable
            .Range(0, 2)
            .Select(_ => TestEntityFactory.CreateRide(driverId: user1.Id).RequiredSuccess())
            .ToList();

        var user2DrivingRides = Enumerable
            .Range(0, 2)
            .Select(_ => TestEntityFactory.CreateRide(driverId: user2.Id).RequiredSuccess())
            .ToList();

        var rides = user1DrivingRides.Concat(user2DrivingRides).ToList();

        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();

        await PersistNewRidePassenger(
            user1DrivingRides
                .Select(r => (r.Id, user2.Id))
                .Concat(user2DrivingRides.Select(r => (r.Id, user1.Id)))
                .ToArray()
        );

        var result = await RideDataLoaders.GetRideIdsByUserIdAsync(
            [user3.Id],
            _dbContext,
            new PagingArguments(first: rides.Count + 1),
            cancellationToken: default
        );

        Assert.False(result.ContainsKey(user3.Id));
    }

    [Fact]
    private async Task RidesWithDifferentTimeOfDeparture_ReturnsOrderedRides()
    {
        var user = TestEntityFactory.CreateUser();
        _dbContext.Users.Add(user);
        var dateTimeOffsetProvider = new FixedDateTimeOffsetProvider
        {
            Now = DateTimeOffset.Now.ToUniversalTime()
        };

        var rnd = new Random(Seed: 42);
        var rides = Enumerable
            .Range(0, 5)
            .Select(i =>
                TestEntityFactory
                    .CreateRide(
                        driverId: user.Id,
                        timeOfDeparture: dateTimeOffsetProvider.Future.AddHours(i),
                        dateTimeOffsetProvider: dateTimeOffsetProvider
                    )
                    .RequiredSuccess()
            )
            .OrderBy(_ => rnd.Next()) // Randomize the order just in case, before the rides get ids from the DB
            .ToList();

        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();

        var result = await RideDataLoaders.GetRideIdsByUserIdAsync(
            [user.Id],
            _dbContext,
            new PagingArguments(first: rides.Count + 1),
            cancellationToken: default
        );

        Assert.Equal(
            rides.OrderBy(r => r.TimeOfDeparture).ThenBy(r => r.Id).Select(r => r.Id),
            result[user.Id].Items.Select(row => row.RideId)
        );
    }

    [Fact]
    private async Task MultipleUsers_ReturnsRidesForEachUser()
    {
        var user1 = TestEntityFactory.CreateUser();
        var user2 = TestEntityFactory.CreateUser();
        _dbContext.Users.AddRange([user1, user2]);
        await _dbContext.SaveChangesAsync();

        var user1DrivingRides = Enumerable
            .Range(0, 2)
            .Select(_ => TestEntityFactory.CreateRide(driverId: user1.Id).RequiredSuccess())
            .ToList();

        var user2DrivingRides = Enumerable
            .Range(0, 3)
            .Select(_ => TestEntityFactory.CreateRide(driverId: user2.Id).RequiredSuccess())
            .ToList();

        var rides = user1DrivingRides.Concat(user2DrivingRides).ToList();

        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();

        await PersistNewRidePassenger(user1DrivingRides.Select(r => (r.Id, user2.Id)).ToArray());

        var result = await RideDataLoaders.GetRideIdsByUserIdAsync(
            [user1.Id, user2.Id],
            _dbContext,
            new PagingArguments(first: rides.Count + 1),
            cancellationToken: default
        );

        Assert.Multiple(() =>
        {
            Assert.Equivalent(
                user1DrivingRides.Select(r => r.Id),
                result[user1.Id].Items.Select(row => row.RideId),
                strict: true
            );
            Assert.Equivalent(
                rides.Select(r => r.Id),
                result[user2.Id].Items.Select(row => row.RideId),
                strict: true
            );
        });
    }

    [Fact]
    private async Task PagingParameters_After_NotSpecified_ReturnsRidesInTheFuture()
    {
        var user = TestEntityFactory.CreateUser();
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // During ride creation, makes it think it's currently 2 days before
        const int creationDayOffset = -2;
        const int currentDayOffset = -creationDayOffset;
        var creationDateTimeOffsetProvider = new FixedDateTimeOffsetProvider
        {
            Now = DateTimeOffset.Now.AddDays(creationDayOffset).ToUniversalTime()
        };

        Ride[] rides =
        [
            TestEntityFactory
                .CreateRide(
                    driverId: user.Id,
                    timeOfDeparture: creationDateTimeOffsetProvider.Now.AddDays(
                        currentDayOffset - 1 // in the past (relative to current time)
                    ),
                    dateTimeOffsetProvider: creationDateTimeOffsetProvider
                )
                .RequiredSuccess(),
            TestEntityFactory
                .CreateRide(
                    driverId: user.Id,
                    timeOfDeparture: creationDateTimeOffsetProvider.Now.AddDays(
                        currentDayOffset + 1 // in the future (relative to current time)
                    ),
                    dateTimeOffsetProvider: creationDateTimeOffsetProvider
                )
                .RequiredSuccess(),
            TestEntityFactory
                .CreateRide(
                    driverId: user.Id,
                    timeOfDeparture: creationDateTimeOffsetProvider.Now.AddDays(
                        currentDayOffset + 1 // in the future (relative to current time)
                    ),
                    dateTimeOffsetProvider: creationDateTimeOffsetProvider
                )
                .RequiredSuccess()
        ];
        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();

        var result = await RideDataLoaders.GetRideIdsByUserIdAsync(
            [user.Id],
            _dbContext,
            new PagingArguments(first: rides.Length + 1),
            cancellationToken: default
        );

        Assert.Equivalent(
            rides
                .Where(r => r.TimeOfDeparture > DateTimeOffset.Now) // Corresponds to CURRENT_TIMESTAMP filter
                .Select(r => r.Id),
            result[user.Id].Items.Select(row => row.RideId),
            strict: true
        );
    }

    [Fact]
    private async Task PagingParameters_After_Respected()
    {
        var user = TestEntityFactory.CreateUser();
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var rides = Enumerable
            .Range(0, 5)
            .Select(_ => TestEntityFactory.CreateRide(driverId: user.Id).RequiredSuccess())
            .ToList();

        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();

        var intermediateResult = await RideDataLoaders.GetRideIdsByUserIdAsync(
            [user.Id],
            _dbContext,
            new PagingArguments(first: rides.Count + 1),
            cancellationToken: default
        );

        Assert.True(intermediateResult.ContainsKey(user.Id));
        var intermediatePage = intermediateResult[user.Id];
        const int skipItemCount = 2;
        var after = intermediatePage.CreateCursor(intermediatePage.Items[skipItemCount - 1]);

        var result = await RideDataLoaders.GetRideIdsByUserIdAsync(
            [user.Id],
            _dbContext,
            new PagingArguments(first: rides.Count + 1, after: after),
            cancellationToken: default
        );

        Assert.Equal(
            intermediatePage
                .Items.Slice(skipItemCount, intermediatePage.Items.Length - skipItemCount)
                .Select(r => r.RideId),
            result[user.Id].Items.Select(row => row.RideId)
        );
    }

    [Fact]
    private async Task PagingParameters_First_Respected()
    {
        var user = TestEntityFactory.CreateUser();
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var rides = Enumerable
            .Range(0, 10)
            .Select(_ => TestEntityFactory.CreateRide(driverId: user.Id).RequiredSuccess())
            .ToList();

        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();

        var resultBelow = await RideDataLoaders.GetRideIdsByUserIdAsync(
            [user.Id],
            _dbContext,
            new PagingArguments(first: rides.Count - 1),
            cancellationToken: default
        );
        var resultAbove = await RideDataLoaders.GetRideIdsByUserIdAsync(
            [user.Id],
            _dbContext,
            new PagingArguments(first: rides.Count + 1),
            cancellationToken: default
        );

        Assert.Multiple(() =>
        {
            Assert.Equal(rides.Count - 1, resultBelow[user.Id].Items.Length);
            Assert.Equal(rides.Count, resultAbove[user.Id].Items.Length);
        });
    }

    [Fact]
    private async Task PageInfo_HasNextPage_Correct()
    {
        var user = TestEntityFactory.CreateUser();
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var rides = Enumerable
            .Range(0, 5)
            .Select(_ => TestEntityFactory.CreateRide(driverId: user.Id).RequiredSuccess())
            .ToList();

        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();

        var resultBelow = await RideDataLoaders.GetRideIdsByUserIdAsync(
            [user.Id],
            _dbContext,
            new PagingArguments(first: rides.Count - 1),
            cancellationToken: default
        );
        var resultExact = await RideDataLoaders.GetRideIdsByUserIdAsync(
            [user.Id],
            _dbContext,
            new PagingArguments(first: rides.Count),
            cancellationToken: default
        );
        var resultAbove = await RideDataLoaders.GetRideIdsByUserIdAsync(
            [user.Id],
            _dbContext,
            new PagingArguments(first: rides.Count + 1),
            cancellationToken: default
        );

        Assert.Multiple(() =>
        {
            Assert.True(resultBelow[user.Id].HasNextPage);
            Assert.False(resultExact[user.Id].HasNextPage);
            Assert.False(resultAbove[user.Id].HasNextPage);
        });
    }
}
