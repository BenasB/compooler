using Compooler.Domain;
using Compooler.Domain.Entities.RideEntity;

namespace Compooler.Application.Commands;

public record RemoveRideCommand(int Id, string UserId);

public class RemoveRideCommandHandler(ICompoolerDbContext dbContext)
    : ICommandHandler<RemoveRideCommand, Ride>
{
    public async Task<Result<Ride>> HandleAsync(
        RemoveRideCommand command,
        CancellationToken ct = default
    )
    {
        var rideToRemove = await dbContext.Rides.FindAsync([command.Id], cancellationToken: ct);

        if (rideToRemove is null)
            return new EntityNotFoundError<Ride, int>(command.Id);

        if (rideToRemove.DriverId != command.UserId)
            return new RideErrors.UserIsNotDriverError();

        dbContext.Rides.Remove(rideToRemove);
        await dbContext.SaveChangesAsync(ct);

        return rideToRemove;
    }
}
