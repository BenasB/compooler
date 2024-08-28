namespace Compooler.Domain.Entities.RideEntity;

public sealed class Ride : IEntity
{
    public int Id { get; }
    public required Route Route { get; init; }
    public required int DriverId { get; init; }

    private readonly List<RidePassenger> _passengers = [];
    public IReadOnlyList<RidePassenger> Passengers => _passengers.AsReadOnly();
    public required int MaxPassengers { get; init; }

    private Ride() { }

    public static Ride Create(Route route, int driverId, int maxPassengers) =>
        new()
        {
            Route = route,
            DriverId = driverId,
            MaxPassengers = maxPassengers
        };

    public Result AddPassenger(int userId)
    {
        if (_passengers.Count >= MaxPassengers)
            return new RideErrors.PassengerLimitReachedError(MaxPassengers);

        if (DriverId == userId)
            return new RideErrors.PassengerIsDriverError(userId);

        var existingPassenger = _passengers.Find(x => x.UserId == userId);
        if (existingPassenger != null)
            return new RideErrors.PassengerAlreadyExistsError(
                existingPassenger.UserId,
                existingPassenger.JoinedAt
            );

        _passengers.Add(RidePassenger.Create(userId: userId));
        return Result.Success();
    }

    public Result RemovePassenger(int userId)
    {
        var passenger = _passengers.Find(p => p.UserId == userId);

        if (passenger == null)
            return new RideErrors.PassengerNotFoundError(userId);

        _passengers.Remove(passenger);
        return Result.Success();
    }
}
