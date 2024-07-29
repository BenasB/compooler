namespace Compooler.Domain.Entities.CommuteGroupEntity;

public sealed class CommuteGroup
{
    public required int Id { get; init; }
    public required Route Route { get; init; }

    public required int DriverId { get; init; }

    private readonly List<int> _passengerIds = [];
    public IReadOnlyList<int> PassengerIds => _passengerIds.AsReadOnly();
    public required int MaxPassengers { get; init; }

    public Result AddPassenger(int userId)
    {
        if (_passengerIds.Count >= MaxPassengers)
            return Result.Failure(CommuteGroupErrors.PassengerLimitReached(this));

        _passengerIds.Add(userId);
        return Result.Success();
    }

    public Result RemovePassenger(int userId)
    {
        if (!_passengerIds.Contains(userId))
            return Result.Failure(CommuteGroupErrors.PassengerNotFound(userId));

        _passengerIds.Remove(userId);
        return Result.Success();
    }
}
