using Compooler.API.Types.Mutations.Inputs;
using Compooler.Application;
using Compooler.Application.Commands;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using JetBrains.Annotations;

namespace Compooler.API.Types.Mutations;

[PublicAPI]
public class RideMutations : ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Mutation);

        descriptor
            .Field("createRide")
            .Authorize()
            .ResolveCompoolerMutation<CreateRideInput, CreateRideCommand, Ride>()
            .Error<GeographicCoordinatesErrors.InvalidLatitudeError>()
            .Error<GeographicCoordinatesErrors.InvalidLongitudeError>()
            .Error<EntityNotFoundError<User, string>>()
            .Error<RideErrors.MaxPassengersBelowOneError>()
            .Error<RideErrors.TimeOfDepartureIsNotInTheFutureError>();

        descriptor
            .Field("removeRide")
            .Authorize()
            .ResolveCompoolerMutation<RemoveRideInput, RemoveRideCommand, Ride>()
            .Error<EntityNotFoundError<Ride, int>>()
            .Error<RideErrors.UserIsNotDriverError>();

        descriptor
            .Field("joinRide")
            .Authorize()
            .ResolveCompoolerMutation<JoinRideInput, JoinRideCommand, Ride>()
            .Error<EntityNotFoundError<Ride, int>>()
            .Error<EntityNotFoundError<User, string>>()
            .Error<RideErrors.PassengerLimitReachedError>()
            .Error<RideErrors.PassengerIsDriverError>()
            .Error<RideErrors.PassengerAlreadyExistsError>();

        descriptor
            .Field("leaveRide")
            .Authorize()
            .ResolveCompoolerMutation<LeaveRideInput, LeaveRideCommand, Ride>()
            .Error<EntityNotFoundError<Ride, int>>()
            .Error<EntityNotFoundError<User, string>>()
            .Error<RideErrors.PassengerNotFoundError>();
    }
}
