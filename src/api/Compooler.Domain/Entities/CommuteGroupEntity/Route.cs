namespace Compooler.Domain.Entities.CommuteGroupEntity;

public sealed class Route
{
    public required GeographicCoordinates Start { get; init; }
    public required GeographicCoordinates Finish { get; init; }
}
