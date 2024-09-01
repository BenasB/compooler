using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;

namespace Compooler.Domain.Tests.Utilities;

public static class TestEntityFactory
{
    private static readonly FixedDateTimeOffsetProvider DateTimeOffsetProvider =
        new() { Now = DateTimeOffset.Now };

    public static User CreateUser(string? firstName = null, string? lastName = null) =>
        User.Create(
            firstName: firstName ?? Guid.NewGuid().ToString("N"),
            lastName: lastName ?? Guid.NewGuid().ToString("N")
        );

    private static Result<GeographicCoordinates> CreateGeographicCoordinates(
        double latitude = 0,
        double longitude = 0
    ) => GeographicCoordinates.Create(latitude: latitude, longitude: longitude);

    public static Result<Ride> CreateRide(
        int maxPassengers = 2,
        int driverId = -42,
        DateTimeOffset? timeOfDeparture = null,
        IDateTimeOffsetProvider? dateTimeOffsetProvider = null
    )
    {
        var startCoords = CreateGeographicCoordinates().RequiredSuccess();
        var finishCoords = CreateGeographicCoordinates().RequiredSuccess();

        return Ride.Create(
            route: Route.Create(start: startCoords, finish: finishCoords),
            driverId: driverId,
            maxPassengers: maxPassengers,
            timeOfDeparture: timeOfDeparture ?? DateTimeOffsetProvider.Future.ToUniversalTime(),
            dateTimeOffsetProvider: dateTimeOffsetProvider ?? DateTimeOffsetProvider
        );
    }

    public static T RequiredSuccess<T>(this Result<T> result)
    {
        if (result.IsFailed)
            throw new InvalidOperationException(
                $"Expected a successful creation of {typeof(T).Name}"
            );

        return result.Value;
    }
}