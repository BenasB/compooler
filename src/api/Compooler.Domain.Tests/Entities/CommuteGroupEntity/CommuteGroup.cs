using Compooler.Domain.Entities.CommuteGroupEntity;

namespace Compooler.Domain.Tests.Entities.CommuteGroupEntity;

public class CommuteGroupTests
{
    private readonly CommuteGroup _commuteGroup;
    private const int MaxPassengers = 2;

    public CommuteGroupTests()
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

        _commuteGroup = CommuteGroup.Create(
            route: Route.Create(start: coords, finish: coords),
            driverId: 0,
            maxPassengers: MaxPassengers
        );
    }

    [Fact]
    public void AddPassenger_LimitReached_Fails()
    {
        for (int i = 0; i < MaxPassengers; i++)
        {
            Assert.False(_commuteGroup.AddPassenger(0).IsFailed);
        }

        var result = _commuteGroup.AddPassenger(MaxPassengers);
        Assert.True(result.IsFailed);
        Assert.Equal(CommuteGroupErrors.PassengerLimitReached(_commuteGroup), result.Error);
    }

    [Fact]
    public void AddPassenger_SpotIsFree_Succeeds()
    {
        var result = _commuteGroup.AddPassenger(0);
        Assert.False(result.IsFailed);
        Assert.Single(_commuteGroup.Passengers);
    }

    [Fact]
    public void RemovePassenger_PassengerNotFound_Fails()
    {
        for (int i = 0; i < MaxPassengers; i++)
        {
            Assert.False(_commuteGroup.AddPassenger(0).IsFailed);
        }

        const int nonExistentUserId = MaxPassengers;
        var result = _commuteGroup.RemovePassenger(nonExistentUserId);
        Assert.True(result.IsFailed);
        Assert.Equal(CommuteGroupErrors.PassengerNotFound(nonExistentUserId), result.Error);
    }

    [Fact]
    public void RemovePassenger_PassengerFound_Succeeds()
    {
        const int userId = 0;
        var result = _commuteGroup.AddPassenger(userId);
        Assert.False(result.IsFailed);

        result = _commuteGroup.RemovePassenger(userId);

        Assert.False(result.IsFailed);
        Assert.Empty(_commuteGroup.Passengers);
    }
}
