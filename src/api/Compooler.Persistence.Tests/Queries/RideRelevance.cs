using Compooler.Domain;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Persistence.Queries;
using Compooler.Tests.Utilities;
using Compooler.Tests.Utilities.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Persistence.Tests.Queries;

[Collection(PersistenceTestsCollection.Name)]
public class RideRelevance(DatabaseFixture fixture) : IAsyncLifetime
{
    private readonly CompoolerDbContext _dbContext = new(fixture.DbContextOptions);

    private static readonly IDateTimeOffsetProvider DateTimeOffsetProvider =
        new FixedDateTimeOffsetProvider { Now = DateTimeOffset.Now.ToUniversalTime() };

    private static readonly (double Latitude, double Longitude) VilniusOzoParkas = (
        Latitude: 54.720017,
        Longitude: 25.280801
    );

    private static readonly (double Latitude, double Longitude) KlaipedaAkropolis = (
        Latitude: 55.694643,
        Longitude: 21.154792
    );

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _dbContext.DisposeAsync().AsTask();

    [Fact]
    public async Task Filtering_StartFarAway_DoesNotIncludeRide()
    {
        var rideStartClose = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates
                    .Create(VilniusOzoParkas.Latitude, VilniusOzoParkas.Longitude)
                    .RequiredSuccess(),
                finishCoords: GeographicCoordinates
                    .Create(KlaipedaAkropolis.Latitude, KlaipedaAkropolis.Longitude)
                    .RequiredSuccess(),
                dateTimeOffsetProvider: DateTimeOffsetProvider
            )
            .RequiredSuccess();

        var rideStartFarAway = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates.Create(56.306018, 22.322808).RequiredSuccess(),
                finishCoords: GeographicCoordinates
                    .Create(KlaipedaAkropolis.Latitude, KlaipedaAkropolis.Longitude)
                    .RequiredSuccess(),
                dateTimeOffsetProvider: DateTimeOffsetProvider
            )
            .RequiredSuccess();

        Ride[] rides = [rideStartClose, rideStartFarAway];

        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();
        var rideIds = rides.Select(x => x.Id).ToList();

        var result = await _dbContext
            .Rides.AsNoTracking()
            .Where(r => rideIds.Contains(r.Id)) // Isolates this test's data from other tests
            .FilterAndOrderByRelevance(
                startLatitude: VilniusOzoParkas.Latitude,
                startLongitude: VilniusOzoParkas.Longitude,
                finishLatitude: KlaipedaAkropolis.Latitude,
                finishLongitude: KlaipedaAkropolis.Longitude,
                userId: TestEntityFactory.CreateUserId(),
                DateTimeOffsetProvider
            )
            .Select(r => r.Id)
            .ToListAsync();

        Assert.Equal([rideStartClose.Id], result);
    }

    [Fact]
    public async Task Filtering_FinishFarAway_DoesNotIncludeRide()
    {
        var rideFinishClose = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates
                    .Create(VilniusOzoParkas.Latitude, VilniusOzoParkas.Longitude)
                    .RequiredSuccess(),
                finishCoords: GeographicCoordinates
                    .Create(KlaipedaAkropolis.Latitude, KlaipedaAkropolis.Longitude)
                    .RequiredSuccess(),
                dateTimeOffsetProvider: DateTimeOffsetProvider
            )
            .RequiredSuccess();

        var rideFinishFarAway = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates
                    .Create(VilniusOzoParkas.Latitude, VilniusOzoParkas.Longitude)
                    .RequiredSuccess(),
                finishCoords: GeographicCoordinates.Create(55.932841, 23.316993).RequiredSuccess(),
                dateTimeOffsetProvider: DateTimeOffsetProvider
            )
            .RequiredSuccess();

        Ride[] rides = [rideFinishClose, rideFinishFarAway];

        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();
        var rideIds = rides.Select(x => x.Id).ToList();

        var result = await _dbContext
            .Rides.AsNoTracking()
            .Where(r => rideIds.Contains(r.Id))
            .FilterAndOrderByRelevance(
                startLatitude: VilniusOzoParkas.Latitude,
                startLongitude: VilniusOzoParkas.Longitude,
                finishLatitude: KlaipedaAkropolis.Latitude,
                finishLongitude: KlaipedaAkropolis.Longitude,
                userId: TestEntityFactory.CreateUserId(),
                DateTimeOffsetProvider
            )
            .Select(r => r.Id)
            .ToListAsync();

        Assert.Equal([rideFinishClose.Id], result);
    }

    [Fact]
    public async Task Filtering_UserIsDriver_DoesNotIncludeRide()
    {
        var user = TestEntityFactory.CreateUser();
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var userIsDriverRide = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates
                    .Create(VilniusOzoParkas.Latitude, VilniusOzoParkas.Longitude)
                    .RequiredSuccess(),
                finishCoords: GeographicCoordinates
                    .Create(KlaipedaAkropolis.Latitude, KlaipedaAkropolis.Longitude)
                    .RequiredSuccess(),
                driverId: user.Id,
                dateTimeOffsetProvider: DateTimeOffsetProvider
            )
            .RequiredSuccess();

        var userIsNotDriverRide = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates
                    .Create(VilniusOzoParkas.Latitude, VilniusOzoParkas.Longitude)
                    .RequiredSuccess(),
                finishCoords: GeographicCoordinates
                    .Create(KlaipedaAkropolis.Latitude, KlaipedaAkropolis.Longitude)
                    .RequiredSuccess(),
                dateTimeOffsetProvider: DateTimeOffsetProvider
            )
            .RequiredSuccess();

        Ride[] rides = [userIsDriverRide, userIsNotDriverRide];

        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();
        var rideIds = rides.Select(x => x.Id).ToList();

        var result = await _dbContext
            .Rides.AsNoTracking()
            .Where(r => rideIds.Contains(r.Id))
            .FilterAndOrderByRelevance(
                startLatitude: VilniusOzoParkas.Latitude,
                startLongitude: VilniusOzoParkas.Longitude,
                finishLatitude: KlaipedaAkropolis.Latitude,
                finishLongitude: KlaipedaAkropolis.Longitude,
                userId: user.Id,
                DateTimeOffsetProvider
            )
            .Select(r => r.Id)
            .ToListAsync();

        Assert.Equal([userIsNotDriverRide.Id], result);
    }

    [Fact]
    public async Task Filtering_RideInThePast_DoesNotIncludeRide()
    {
        var creationDateTimeOffsetProvider = new FixedDateTimeOffsetProvider
        {
            Now = DateTimeOffset.MinValue.ToUniversalTime()
        };

        var queryDateTimeOffsetProvider = new FixedDateTimeOffsetProvider
        {
            Now = DateTimeOffset.Now.ToUniversalTime()
        };

        var start = VilniusOzoParkas;
        var finish = KlaipedaAkropolis;

        var rideInThePast = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates
                    .Create(start.Latitude, start.Longitude)
                    .RequiredSuccess(),
                finishCoords: GeographicCoordinates
                    .Create(finish.Latitude, finish.Longitude)
                    .RequiredSuccess(),
                timeOfDeparture: queryDateTimeOffsetProvider.Past,
                dateTimeOffsetProvider: creationDateTimeOffsetProvider
            )
            .RequiredSuccess();

        var rideInTheFuture = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates
                    .Create(start.Latitude, start.Longitude)
                    .RequiredSuccess(),
                finishCoords: GeographicCoordinates
                    .Create(finish.Latitude, finish.Longitude)
                    .RequiredSuccess(),
                timeOfDeparture: queryDateTimeOffsetProvider.Future,
                dateTimeOffsetProvider: creationDateTimeOffsetProvider
            )
            .RequiredSuccess();

        Ride[] rides = [rideInThePast, rideInTheFuture];

        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();
        var rideIds = rides.Select(x => x.Id).ToList();

        var result = await _dbContext
            .Rides.AsNoTracking()
            .Where(r => rideIds.Contains(r.Id))
            .FilterAndOrderByRelevance(
                startLatitude: start.Latitude,
                startLongitude: start.Longitude,
                finishLatitude: finish.Latitude,
                finishLongitude: finish.Longitude,
                userId: TestEntityFactory.CreateUserId(),
                queryDateTimeOffsetProvider
            )
            .Select(r => r.Id)
            .ToListAsync();

        Assert.Equal([rideInTheFuture.Id], result);
    }

    [Fact]
    public async Task Ranking_Start_CloserPreferred()
    {
        var ride = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates.Create(54.715139, 25.275054).RequiredSuccess(),
                finishCoords: GeographicCoordinates
                    .Create(KlaipedaAkropolis.Latitude, KlaipedaAkropolis.Longitude)
                    .RequiredSuccess(),
                dateTimeOffsetProvider: DateTimeOffsetProvider
            )
            .RequiredSuccess();

        var closerStartRide = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates
                    .Create(VilniusOzoParkas.Latitude, VilniusOzoParkas.Longitude)
                    .RequiredSuccess(),
                finishCoords: GeographicCoordinates
                    .Create(KlaipedaAkropolis.Latitude, KlaipedaAkropolis.Longitude)
                    .RequiredSuccess(),
                dateTimeOffsetProvider: DateTimeOffsetProvider
            )
            .RequiredSuccess();

        Ride[] rides = [ride, closerStartRide];

        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();
        var rideIds = rides.Select(x => x.Id).ToList();

        var result = await _dbContext
            .Rides.AsNoTracking()
            .Where(r => rideIds.Contains(r.Id))
            .FilterAndOrderByRelevance(
                startLatitude: VilniusOzoParkas.Latitude,
                startLongitude: VilniusOzoParkas.Longitude,
                finishLatitude: KlaipedaAkropolis.Latitude,
                finishLongitude: KlaipedaAkropolis.Longitude,
                userId: TestEntityFactory.CreateUserId(),
                DateTimeOffsetProvider
            )
            .Select(r => r.Id)
            .ToListAsync();

        Assert.Equal([closerStartRide.Id, ride.Id], result);
    }

    [Fact]
    public async Task Ranking_Finish_CloserPreferred()
    {
        var ride = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates
                    .Create(VilniusOzoParkas.Latitude, VilniusOzoParkas.Longitude)
                    .RequiredSuccess(),
                finishCoords: GeographicCoordinates.Create(55.687824, 21.153214).RequiredSuccess(),
                dateTimeOffsetProvider: DateTimeOffsetProvider
            )
            .RequiredSuccess();

        var closerFinishRide = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates
                    .Create(VilniusOzoParkas.Latitude, VilniusOzoParkas.Longitude)
                    .RequiredSuccess(),
                finishCoords: GeographicCoordinates
                    .Create(KlaipedaAkropolis.Latitude, KlaipedaAkropolis.Longitude)
                    .RequiredSuccess(),
                dateTimeOffsetProvider: DateTimeOffsetProvider
            )
            .RequiredSuccess();

        Ride[] rides = [ride, closerFinishRide];

        _dbContext.Rides.AddRange(rides);
        await _dbContext.SaveChangesAsync();
        var rideIds = rides.Select(x => x.Id).ToList();

        var result = await _dbContext
            .Rides.AsNoTracking()
            .Where(r => rideIds.Contains(r.Id))
            .FilterAndOrderByRelevance(
                startLatitude: VilniusOzoParkas.Latitude,
                startLongitude: VilniusOzoParkas.Longitude,
                finishLatitude: KlaipedaAkropolis.Latitude,
                finishLongitude: KlaipedaAkropolis.Longitude,
                userId: TestEntityFactory.CreateUserId(),
                DateTimeOffsetProvider
            )
            .Select(r => r.Id)
            .ToListAsync();

        Assert.Equal([closerFinishRide.Id, ride.Id], result);
    }
}
