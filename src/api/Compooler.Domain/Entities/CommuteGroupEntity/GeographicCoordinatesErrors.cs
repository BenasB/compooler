namespace Compooler.Domain.Entities.CommuteGroupEntity;

public static class GeographicCoordinatesErrors
{
    public record InvalidLatitudeError()
        : Error(
            "GeographicCoordinates.Create.InvalidLatitude",
            "Latitude value must be between -90 and 90"
        );

    public record InvalidLongitudeError()
        : Error(
            "GeographicCoordinates.Create.InvalidLongitude",
            "Longitude value must be between -180 and 180"
        );
}
