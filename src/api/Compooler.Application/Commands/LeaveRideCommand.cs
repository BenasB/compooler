using Compooler.Domain;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;

namespace Compooler.Application.Commands;

public record LeaveRideCommand(int RideId, string UserId);

public class LeaveRideCommandHandler(ICompoolerDbContext dbContext)
    : ICommandHandler<LeaveRideCommand, Ride>
{
    public async Task<Result<Ride>> HandleAsync(
        LeaveRideCommand command,
        CancellationToken ct = default
    )
    {
        var ride = await dbContext.Rides.FindAsync([command.RideId], cancellationToken: ct);

        if (ride is null)
            return new EntityNotFoundError<Ride, int>(command.RideId);

        var user = await dbContext.Users.FindAsync([command.UserId], cancellationToken: ct);

        if (user is null)
            return new EntityNotFoundError<User, string>(command.UserId);

        var result = ride.RemovePassenger(user.Id);

        if (result.IsFailed)
            return result.Error;

        await dbContext.SaveChangesAsync(ct);
        return ride;
    }
}
