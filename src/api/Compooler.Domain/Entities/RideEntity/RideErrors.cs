namespace Compooler.Domain.Entities.RideEntity;

public static class RideErrors
{
    public record PassengerLimitReachedError(int MaxPassengers)
        : Error("Ride.AddPassenger.PassengerLimitReached", "Reached passenger limit");

    public record PassengerNotFoundError(int UserId)
        : Error("Ride.AddPassenger.PassengerNotFound", "User not found in the ride's passengers");
}
