using Compooler.Domain;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence.Configurations;

namespace Compooler.Tests.Utilities;

public static class TestEntityFactory
{
    private static readonly FixedDateTimeOffsetProvider DateTimeOffsetProvider =
        new() { Now = DateTimeOffset.Now.ToUniversalTime() };

    public static string CreateUserId() =>
        Guid.NewGuid().ToString("N")[..UserConfiguration.IdLength];

    public static User CreateUser(string? firstName = null, string? lastName = null) =>
        User.Create(
            id: CreateUserId(),
            firstName: firstName ?? Guid.NewGuid().ToString("N"),
            lastName: lastName ?? Guid.NewGuid().ToString("N")
        );

    private static Result<GeographicCoordinates> CreateGeographicCoordinates(
        double latitude = 0,
        double longitude = 0
    ) => GeographicCoordinates.Create(latitude: latitude, longitude: longitude);

    public static Result<Ride> CreateRide(
        int maxPassengers = 2,
        string? driverId = null,
        GeographicCoordinates? startCoords = null,
        GeographicCoordinates? finishCoords = null,
        DateTimeOffset? timeOfDeparture = null,
        IDateTimeOffsetProvider? dateTimeOffsetProvider = null
    )
    {
        startCoords ??= CreateGeographicCoordinates().RequiredSuccess();
        finishCoords ??= CreateGeographicCoordinates().RequiredSuccess();

        return Ride.Create(
            route: Route.Create(start: startCoords, finish: finishCoords),
            driverId: driverId ?? TestEntityFactory.CreateUserId(),
            maxPassengers: maxPassengers,
            timeOfDeparture: timeOfDeparture
                ?? dateTimeOffsetProvider?.Now.AddDays(1)
                ?? DateTimeOffsetProvider.Future,
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
