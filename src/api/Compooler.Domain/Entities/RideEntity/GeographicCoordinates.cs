namespace Compooler.Domain.Entities.RideEntity;

public sealed class GeographicCoordinates
{
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }

    private GeographicCoordinates() { }

    public static Result<GeographicCoordinates> Create(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
            return new GeographicCoordinatesErrors.InvalidLatitudeError();

        if (longitude is < -180 or > 180)
            return new GeographicCoordinatesErrors.InvalidLongitudeError();

        return new GeographicCoordinates { Latitude = latitude, Longitude = longitude };
    }
}
