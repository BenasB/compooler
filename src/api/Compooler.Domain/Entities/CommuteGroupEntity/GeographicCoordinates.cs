namespace Compooler.Domain.Entities.CommuteGroupEntity;

public sealed class GeographicCoordinates
{
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }

    private GeographicCoordinates() { }

    public static Result<GeographicCoordinates> Create(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
            return Result<GeographicCoordinates>.Failure(
                GeographicCoordinatesErrors.InvalidLatitude
            );

        if (longitude is < -180 or > 180)
            return Result<GeographicCoordinates>.Failure(
                GeographicCoordinatesErrors.InvalidLongitude
            );

        return Result<GeographicCoordinates>.Success(
            new GeographicCoordinates { Latitude = latitude, Longitude = longitude }
        );
    }
}
