namespace Compooler.Domain.Entities.RideEntity;

public static class RideErrors
{
    public record PassengerLimitReachedError(int MaxPassengers)
        : Error("Ride.AddPassenger.PassengerLimitReached", "Reached passenger limit");

    public record PassengerNotFoundError(string UserId)
        : Error("Ride.AddPassenger.PassengerNotFound", "User not found in the ride's passengers");

    public record PassengerIsDriverError(string DriverId)
        : Error(
            "Ride.AddPassenger.PassengerIsDriverError",
            "The ride's driver can't be a passenger"
        );

    public record PassengerAlreadyExistsError(string PassengerId, DateTimeOffset JoinedAt)
        : Error(
            "Ride.AddPassenger.PassengerAlreadyExistsError",
            "User is already a passenger in this ride"
        );

    public record TimeOfDepartureIsNotInTheFutureError(
        DateTimeOffset TimeOfDeparture,
        DateTimeOffset Now
    )
        : Error(
            "Ride.Create.TimeOfDepartureIsNotInTheFutureError",
            "The ride's planned time of departure must be in the future"
        );

    public record MaxPassengersBelowOneError(int MaxPassengers)
        : Error(
            "Ride.Create.MaxPassengersBelowOneError",
            "The number of maximum passengers in the ride must be at least 1"
        );
}
