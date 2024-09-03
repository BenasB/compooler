using NetTopologySuite.Geometries;

namespace Compooler.Domain.Entities.RideEntity;

public sealed class GeographicCoordinates
{
    public double Latitude => Point.Y;
    public double Longitude => Point.X;

    /// <summary>
    /// Backing type to allow spatial queries
    /// </summary>
    private Point Point { get; init; } = Point.Empty;

    private GeographicCoordinates() { }

    public static Result<GeographicCoordinates> Create(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
            return new GeographicCoordinatesErrors.InvalidLatitudeError();

        if (longitude is < -180 or > 180)
            return new GeographicCoordinatesErrors.InvalidLongitudeError();

        return new GeographicCoordinates { Point = new Point(x: longitude, y: latitude) };
    }
}
