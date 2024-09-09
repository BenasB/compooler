using Compooler.Domain.Entities.RideEntity;
using Compooler.Tests.Utilities;

namespace Compooler.Domain.Tests.Entities.RideEntity;

public class RideTests
{
    [Fact]
    public void Create_TimeOfDepartureInThePast_Fails()
    {
        var dateTimeOffsetProvider = new FixedDateTimeOffsetProvider { Now = DateTimeOffset.Now };
        var result = TestEntityFactory.CreateRide(
            timeOfDeparture: dateTimeOffsetProvider.Past,
            dateTimeOffsetProvider: dateTimeOffsetProvider
        );

        Assert.True(result.IsFailed);
        Assert.Equal(
            new RideErrors.TimeOfDepartureIsNotInTheFutureError(
                dateTimeOffsetProvider.Past,
                dateTimeOffsetProvider.Now
            ),
            result.Error
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_MaxPassengersBelowOne_Fails(int maxPassengers)
    {
        var result = TestEntityFactory.CreateRide(maxPassengers: maxPassengers);

        Assert.True(result.IsFailed);
        Assert.Equal(new RideErrors.MaxPassengersBelowOneError(maxPassengers), result.Error);
    }

    [Fact]
    public void AddPassenger_LimitReached_Fails()
    {
        const int maxPassengers = 2;
        var ride = TestEntityFactory.CreateRide(maxPassengers: maxPassengers).RequiredSuccess();

        for (int i = 0; i < maxPassengers; i++)
        {
            Assert.False(ride.AddPassenger(TestEntityFactory.CreateUserId()).IsFailed);
        }

        var result = ride.AddPassenger(TestEntityFactory.CreateUserId());
        Assert.True(result.IsFailed);
        Assert.Equal(new RideErrors.PassengerLimitReachedError(maxPassengers), result.Error);
    }

    [Fact]
    public void AddPassenger_PassengerIsDriver_Fails()
    {
        var driverId = TestEntityFactory.CreateUserId();
        var ride = TestEntityFactory.CreateRide(driverId: driverId).RequiredSuccess();
        var result = ride.AddPassenger(driverId);
        Assert.True(result.IsFailed);
        Assert.Equal(new RideErrors.PassengerIsDriverError(driverId), result.Error);
    }

    [Fact]
    public void AddPassenger_PassengerAlreadyExists_Fails()
    {
        var ride = TestEntityFactory.CreateRide(maxPassengers: 3).RequiredSuccess();
        var passengerId = TestEntityFactory.CreateUserId();

        var result = ride.AddPassenger(passengerId);
        Assert.False(result.IsFailed);

        result = ride.AddPassenger(passengerId);
        Assert.True(result.IsFailed);
        Assert.Equal(
            new RideErrors.PassengerAlreadyExistsError(passengerId, default),
            result.Error
        );
    }

    [Fact]
    public void AddPassenger_SpotIsFree_Succeeds()
    {
        var ride = TestEntityFactory.CreateRide().RequiredSuccess();

        var result = ride.AddPassenger(TestEntityFactory.CreateUserId());
        Assert.False(result.IsFailed);
        Assert.Single(ride.Passengers);
    }

    [Fact]
    public void RemovePassenger_PassengerNotFound_Fails()
    {
        const int maxPassengers = 2;
        var ride = TestEntityFactory.CreateRide(maxPassengers: maxPassengers).RequiredSuccess();

        for (int i = 0; i < maxPassengers; i++)
        {
            Assert.False(ride.AddPassenger(TestEntityFactory.CreateUserId()).IsFailed);
        }

        var notPassengerId = TestEntityFactory.CreateUserId();
        var result = ride.RemovePassenger(notPassengerId);
        Assert.True(result.IsFailed);
        Assert.Equal(new RideErrors.PassengerNotFoundError(notPassengerId), result.Error);
    }

    [Fact]
    public void RemovePassenger_PassengerFound_Succeeds()
    {
        var ride = TestEntityFactory.CreateRide().RequiredSuccess();

        var userId = TestEntityFactory.CreateUserId();
        var result = ride.AddPassenger(userId);
        Assert.False(result.IsFailed);

        result = ride.RemovePassenger(userId);

        Assert.False(result.IsFailed);
        Assert.Empty(ride.Passengers);
    }
}
