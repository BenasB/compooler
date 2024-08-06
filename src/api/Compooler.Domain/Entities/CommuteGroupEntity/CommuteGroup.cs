namespace Compooler.Domain.Entities.CommuteGroupEntity;

public sealed class CommuteGroup : IEntity
{
    public int Id { get; }
    public required Route Route { get; init; }
    public required int DriverId { get; init; }

    private readonly List<CommuteGroupPassenger> _passengers = [];
    public IReadOnlyList<CommuteGroupPassenger> Passengers => _passengers.AsReadOnly();
    public required int MaxPassengers { get; init; }

    private CommuteGroup() { }

    public static CommuteGroup Create(Route route, int driverId, int maxPassengers) =>
        new()
        {
            Route = route,
            DriverId = driverId,
            MaxPassengers = maxPassengers
        };

    public Result AddPassenger(int userId)
    {
        if (_passengers.Count >= MaxPassengers)
            return Result.Failure(new CommuteGroupErrors.PassengerLimitReachedError(MaxPassengers));

        _passengers.Add(CommuteGroupPassenger.Create(userId: userId));
        return Result.Success();
    }

    public Result RemovePassenger(int userId)
    {
        var passenger = _passengers.Find(p => p.UserId == userId);

        if (passenger == null)
            return Result.Failure(new CommuteGroupErrors.PassengerNotFoundError(userId));

        _passengers.Remove(passenger);
        return Result.Success();
    }
}
