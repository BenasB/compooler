using Compooler.Domain;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;

namespace Compooler.Application.Commands;

public record CreateRideCommand(
    int DriverId,
    int MaxPassengers,
    double StartLatitude,
    double StartLongitude,
    double FinishLatitude,
    double FinishLongitude
);

public class CreateRideCommandHandler(ICompoolerDbContext dbContext)
    : ICommandHandler<CreateRideCommand, Ride>
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
        {
            return new EntityNotFoundError<User>(command.DriverId);
        }

        var route = Route.Create(startResult.Value, finishResult.Value);
        var ride = Ride.Create(route, command.DriverId, command.MaxPassengers);

        dbContext.Rides.Add(ride);
        await dbContext.SaveChangesAsync(ct);

        return ride;
    }
}
