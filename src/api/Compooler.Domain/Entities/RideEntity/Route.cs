namespace Compooler.Domain.Entities.RideEntity;

public sealed class Route
{
    public required GeographicCoordinates Start { get; init; }
    public required GeographicCoordinates Finish { get; init; }

    private Route() { }

    public static Route Create(GeographicCoordinates start, GeographicCoordinates finish) =>
        new() { Start = start, Finish = finish };
}
