using Compooler.Application.Commands;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using HotChocolate.Resolvers;
using JetBrains.Annotations;

namespace Compooler.API.Types.Mutations.Inputs;

[PublicAPI]
public record CreateRideInput(
    [property: ID<User>] string DriverId,
    int MaxPassengers,
    double StartLatitude,
    double StartLongitude,
    double FinishLatitude,
    double FinishLongitude,
    DateTimeOffset TimeOfDeparture
) : IMappableTo<CreateRideCommand>
{
    public CreateRideCommand Map(IResolverContext ctx) =>
        new(
            DriverId: DriverId,
            MaxPassengers: MaxPassengers,
            StartLatitude: StartLatitude,
            StartLongitude: StartLongitude,
            FinishLatitude: FinishLatitude,
            FinishLongitude: FinishLongitude,
            TimeOfDeparture: TimeOfDeparture.ToUniversalTime()
        );
}

[PublicAPI]
public record RemoveRideInput([property: ID<Ride>] int Id) : IMappableTo<RemoveRideCommand>
{
    public RemoveRideCommand Map(IResolverContext ctx) => new(Id: Id);
}

[PublicAPI]
public record JoinRideInput([property: ID<Ride>] int RideId, [property: ID<User>] string UserId)
    : IMappableTo<JoinRideCommand>
{
    public JoinRideCommand Map(IResolverContext ctx) => new(RideId: RideId, UserId: UserId);
}

[PublicAPI]
public record LeaveRideInput([property: ID<Ride>] int RideId, [property: ID<User>] string UserId)
    : IMappableTo<LeaveRideCommand>
{
    public LeaveRideCommand Map(IResolverContext ctx) => new(RideId: RideId, UserId: UserId);
}
