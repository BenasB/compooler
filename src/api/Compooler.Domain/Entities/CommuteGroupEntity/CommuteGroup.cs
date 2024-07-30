namespace Compooler.Domain.Entities.CommuteGroupEntity;

public sealed class CommuteGroup
{
    public required int Id { get; init; }
    public required Route Route { get; init; }

    public required int DriverId { get; init; }

    private readonly List<GroupPassenger> _passengers = [];
    public IReadOnlyList<GroupPassenger> Passengers => _passengers.AsReadOnly();
    public required int MaxPassengers { get; init; }

    public Result AddPassenger(int userId)
    {
        if (_passengers.Count >= MaxPassengers)
            return Result.Failure(CommuteGroupErrors.PassengerLimitReached(this));

        _passengers.Add(new GroupPassenger { UserId = userId });
        return Result.Success();
    }

    public Result RemovePassenger(int userId)
    {
        var passenger = _passengers.Find(p => p.UserId == userId);

        if (passenger == null)
            return Result.Failure(CommuteGroupErrors.PassengerNotFound(userId));

        _passengers.Remove(passenger);
        return Result.Success();
    }
}
