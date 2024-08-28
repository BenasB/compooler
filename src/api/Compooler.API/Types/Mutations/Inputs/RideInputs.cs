using Compooler.Application.Commands;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using JetBrains.Annotations;

namespace Compooler.API.Types.Mutations.Inputs;

[PublicAPI]
public record CreateRideInput(
    [property: ID<User>] int DriverId,
    int MaxPassengers,
    double StartLatitude,
    double StartLongitude,
    double FinishLatitude,
    double FinishLongitude
) : IMappableTo<CreateRideCommand>
{
    public CreateRideCommand Map() =>
        new(
            DriverId: DriverId,
            MaxPassengers: MaxPassengers,
            StartLatitude: StartLatitude,
            StartLongitude: StartLongitude,
            FinishLatitude: FinishLatitude,
            FinishLongitude: FinishLongitude
        );
}

[PublicAPI]
public record RemoveRideInput([property: ID<Ride>] int Id) : IMappableTo<RemoveRideCommand>
{
    public RemoveRideCommand Map() => new(Id: Id);
}

[PublicAPI]
public record JoinRideInput([property: ID<Ride>] int RideId, [property: ID<User>] int UserId)
    : IMappableTo<JoinRideCommand>
{
    public JoinRideCommand Map() => new(RideId: RideId, UserId: UserId);
}

[PublicAPI]
public record LeaveRideInput([property: ID<Ride>] int RideId, [property: ID<User>] int UserId)
    : IMappableTo<LeaveRideCommand>
{
    public LeaveRideCommand Map() => new(RideId: RideId, UserId: UserId);
}
