namespace Compooler.Domain.Entities.CommuteGroupEntity;

public static class GeographicCoordinatesErrors
{
    public static readonly Error InvalidLatitude =
        new(
            "GeographicCoordinates.Create.InvalidLatitude",
            "Latitude value must be between -90 and 90"
        );

    public static readonly Error InvalidLongitude =
        new(
            "GeographicCoordinates.Create.InvalidLongitude",
            "Longitude value must be between -180 and 180"
        );
}
