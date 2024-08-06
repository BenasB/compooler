using Compooler.Domain;
using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Domain.Entities.UserEntity;

namespace Compooler.Application.Commands;

public record CreateCommuteGroupCommand(
    int DriverId,
    int MaxPassengers,
    double StartLatitude,
    double StartLongitude,
    double FinishLatitude,
    double FinishLongitude
);

public class CreateCommuteGroupHandler(ICompoolerDbContext dbContext)
    : ICommandHandler<CreateCommuteGroupCommand, CommuteGroup>
{
    public async Task<Result<CommuteGroup>> HandleAsync(
        CreateCommuteGroupCommand command,
        CancellationToken ct
    )
    {
        var startResult = GeographicCoordinates.Create(
            command.StartLatitude,
            command.StartLongitude
        );

        if (startResult.IsFailed)
            return Result<CommuteGroup>.Failure(startResult.Error);

        var finishResult = GeographicCoordinates.Create(
            command.FinishLatitude,
            command.FinishLongitude
        );

        if (finishResult.IsFailed)
            return Result<CommuteGroup>.Failure(finishResult.Error);

        var driver = await dbContext.Users.FindAsync([command.DriverId], cancellationToken: ct);

        if (driver is null)
        {
            return Result<CommuteGroup>.Failure(new EntityNotFoundError<User>(command.DriverId));
        }

        var route = Route.Create(startResult.Value, finishResult.Value);
        var commuteGroup = CommuteGroup.Create(route, command.DriverId, command.MaxPassengers);

        dbContext.CommuteGroups.Add(commuteGroup);
        await dbContext.SaveChangesAsync(ct);

        return Result<CommuteGroup>.Success(commuteGroup);
    }
}
