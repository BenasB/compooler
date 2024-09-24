using Compooler.API.Extensions;
using Compooler.Application.Commands;
using Compooler.Domain.Entities.RideEntity;
using HotChocolate.Resolvers;
using JetBrains.Annotations;

namespace Compooler.API.Types.Mutations.Inputs;

[PublicAPI]
public record CreateRideInput(
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
            DriverId: ctx.GetRequiredUserId(),
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
    public RemoveRideCommand Map(IResolverContext ctx) =>
        new(Id: Id, UserId: ctx.GetRequiredUserId());
}

[PublicAPI]
public record JoinRideInput([property: ID<Ride>] int RideId) : IMappableTo<JoinRideCommand>
{
    public JoinRideCommand Map(IResolverContext ctx) =>
        new(RideId: RideId, UserId: ctx.GetRequiredUserId());
}

[PublicAPI]
public record LeaveRideInput([property: ID<Ride>] int RideId) : IMappableTo<LeaveRideCommand>
{
    public LeaveRideCommand Map(IResolverContext ctx) =>
        new(RideId: RideId, UserId: ctx.GetRequiredUserId());
}
