using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Tests.Utilities;
using Compooler.Persistence.Queries;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Persistence.Tests.Queries;

[Collection(PersistenceTestsCollection.Name)]
public class RideRelevance(DatabaseFixture fixture) : IAsyncLifetime
{
    private readonly CompoolerDbContext _dbContext = new(fixture.DbContextOptions);

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _dbContext.DisposeAsync().AsTask();

    [Fact]
    public async Task CorrectRidesAndOrder()
    {
        var vilniusAkropolisToMazeikiai = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates.Create(54.711368, 25.261237).RequiredSuccess(),
                finishCoords: GeographicCoordinates.Create(56.303637, 22.338049).RequiredSuccess()
            )
            .RequiredSuccess();

        var mazeikiaiToSiauliai = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates.Create(56.306018, 22.322808).RequiredSuccess(),
                finishCoords: GeographicCoordinates.Create(55.932841, 23.316993).RequiredSuccess()
            )
            .RequiredSuccess();

        var vilniusGariunaiToKlaipedaOldPort = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates.Create(54.660485, 25.159930).RequiredSuccess(),
                finishCoords: GeographicCoordinates.Create(55.706472, 21.123317).RequiredSuccess()
            )
            .RequiredSuccess();

        var klaipedaAkropolisToVilniusAkropolis = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates.Create(55.698628, 21.148453).RequiredSuccess(),
                finishCoords: GeographicCoordinates.Create(54.711878, 25.276196).RequiredSuccess()
            )
            .RequiredSuccess();

        var vilniusAkropolisToKlaipedaAkropolis = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates.Create(54.711368, 25.261237).RequiredSuccess(),
                finishCoords: GeographicCoordinates.Create(55.690882, 21.156413).RequiredSuccess()
            )
            .RequiredSuccess();

        var vilniusToKaunas = TestEntityFactory
            .CreateRide(
                startCoords: GeographicCoordinates.Create(54.711787, 25.301676).RequiredSuccess(),
                finishCoords: GeographicCoordinates.Create(54.902804, 23.917552).RequiredSuccess()
            )
            .RequiredSuccess();

        _dbContext.Rides.AddRange(
            [
                vilniusAkropolisToMazeikiai,
                mazeikiaiToSiauliai,
                vilniusGariunaiToKlaipedaOldPort,
                klaipedaAkropolisToVilniusAkropolis,
                vilniusAkropolisToKlaipedaAkropolis,
                vilniusToKaunas
            ]
        );

        await _dbContext.SaveChangesAsync();

        var result = await _dbContext
            .Rides.AsNoTracking()
            .FilterAndOrderByRelevance(
                startLatitude: 54.720017, // Vilnius Ozo parkas
                startLongitude: 25.280801,
                finishLatitude: 55.694643, // Klaipeda Akropolis
                finishLongitude: 21.154792
            )
            .ToListAsync();

        Assert.Equal(
            new[] { vilniusAkropolisToKlaipedaAkropolis, vilniusGariunaiToKlaipedaOldPort }.Select(
                x => x.Id
            ),
            result.Select(x => x.Id)
        );
    }
}
