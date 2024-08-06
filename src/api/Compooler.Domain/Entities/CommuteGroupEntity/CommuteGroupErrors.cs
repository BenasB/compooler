namespace Compooler.Domain.Entities.CommuteGroupEntity;

public static class CommuteGroupErrors
{
    public record PassengerLimitReachedError(int MaxPassengers)
        : Error("CommuteGroup.AddPassenger.PassengerLimitReached", "Reached passenger limit");

    public record PassengerNotFoundError(int UserId)
        : Error(
            "CommuteGroup.AddPassenger.PassengerNotFound",
            "User not found in the group's passengers"
        );
}
