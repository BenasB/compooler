using Compooler.Domain.Entities.RideEntity;

namespace Compooler.Domain.Tests.Entities.RideEntity;

public class RideTests
{
    private readonly Ride _ride;
    private const int MaxPassengers = 2;
    private const int DriverId = -42;

    public RideTests()
    {
        var coordsResult = GeographicCoordinates.Create(0, 0);
        var coords = coordsResult.IsFailed switch
        {
            false => coordsResult.Value,
            true
                => throw new InvalidOperationException(
                    "Expected a successful creation of coordinates"
                )
        };

        _ride = Ride.Create(
            route: Route.Create(start: coords, finish: coords),
            driverId: DriverId,
            maxPassengers: MaxPassengers,
            leaveTime: DateTimeOffset.Now.AddDays(1)
        );
    }

    [Fact]
    public void AddPassenger_LimitReached_Fails()
    {
        for (int i = 0; i < MaxPassengers; i++)
        {
            Assert.False(_ride.AddPassenger(i).IsFailed);
        }

        var result = _ride.AddPassenger(MaxPassengers);
        Assert.True(result.IsFailed);
        Assert.Equal(new RideErrors.PassengerLimitReachedError(MaxPassengers), result.Error);
    }

    [Fact]
    public void AddPassenger_PassengerIsDriver_Fails()
    {
        var result = _ride.AddPassenger(DriverId);
        Assert.True(result.IsFailed);
        Assert.Equal(new RideErrors.PassengerIsDriverError(DriverId), result.Error);
    }

    [Fact]
    public void AddPassenger_PassengerAlreadyExists_Fails()
    {
        var result = _ride.AddPassenger(0);
        Assert.False(result.IsFailed);

        result = _ride.AddPassenger(0);
        Assert.True(result.IsFailed);
        Assert.Equal(new RideErrors.PassengerAlreadyExistsError(0, default), result.Error);
    }

    [Fact]
    public void AddPassenger_SpotIsFree_Succeeds()
    {
        var result = _ride.AddPassenger(0);
        Assert.False(result.IsFailed);
        Assert.Single(_ride.Passengers);
    }

    [Fact]
    public void RemovePassenger_PassengerNotFound_Fails()
    {
        for (int i = 0; i < MaxPassengers; i++)
        {
            Assert.False(_ride.AddPassenger(i).IsFailed);
        }

        const int nonExistentUserId = MaxPassengers;
        var result = _ride.RemovePassenger(nonExistentUserId);
        Assert.True(result.IsFailed);
        Assert.Equal(new RideErrors.PassengerNotFoundError(nonExistentUserId), result.Error);
    }

    [Fact]
    public void RemovePassenger_PassengerFound_Succeeds()
    {
        const int userId = 0;
        var result = _ride.AddPassenger(userId);
        Assert.False(result.IsFailed);

        result = _ride.RemovePassenger(userId);

        Assert.False(result.IsFailed);
        Assert.Empty(_ride.Passengers);
    }
}
