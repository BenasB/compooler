namespace Compooler.Domain.Entities.CommuteGroupEntity;

public static class CommuteGroupErrors
{
    public static Error PassengerLimitReached(CommuteGroup group) =>
        new(
            "CommuteGroup.AddPassenger.PassengerLimitReached",
            $"Reached passenger limit '{group.MaxPassengers}'"
        );

    public static Error PassengerNotFound(int userId) =>
        new(
            "CommuteGroup.RemovePassenger.PassengerNotFound",
            $"User '{userId}' not found in the group's passengers"
        );
}
