using Compooler.Domain;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;

namespace Compooler.Application.Commands;

public record CreateRideCommand(
    string DriverId,
    int MaxPassengers,
    double StartLatitude,
    double StartLongitude,
    double FinishLatitude,
    double FinishLongitude,
    DateTimeOffset TimeOfDeparture
);

public class CreateRideCommandHandler(
    ICompoolerDbContext dbContext,
    IDateTimeOffsetProvider dateTimeOffsetProvider
) : ICommandHandler<CreateRideCommand, Ride>
{
    public async Task<Result<Ride>> HandleAsync(
        CreateRideCommand command,
        CancellationToken ct = default
    )
    {
        var startResult = GeographicCoordinates.Create(
            command.StartLatitude,
            command.StartLongitude
        );

        if (startResult.IsFailed)
            return startResult.Error;

        var finishResult = GeographicCoordinates.Create(
            command.FinishLatitude,
            command.FinishLongitude
        );

        if (finishResult.IsFailed)
            return finishResult.Error;

        var driver = await dbContext.Users.FindAsync([command.DriverId], cancellationToken: ct);

        if (driver is null)
            return new EntityNotFoundError<User, string>(command.DriverId);

        var route = Route.Create(startResult.Value, finishResult.Value);
        var rideResult = Ride.Create(
            route,
            command.DriverId,
            command.MaxPassengers,
            command.TimeOfDeparture,
            dateTimeOffsetProvider
        );

        if (rideResult.IsFailed)
            return rideResult.Error;

        dbContext.Rides.Add(rideResult.Value);
        await dbContext.SaveChangesAsync(ct);

        return rideResult.Value;
    }
}
