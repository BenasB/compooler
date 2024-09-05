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
            .ResolveCompoolerMutation<CreateRideInput, CreateRideCommand, Ride>()
            .Error<GeographicCoordinatesErrors.InvalidLatitudeError>()
            .Error<GeographicCoordinatesErrors.InvalidLongitudeError>()
            .Error<EntityNotFoundError<User, string>>()
            .Error<RideErrors.MaxPassengersBelowOneError>()
            .Error<RideErrors.TimeOfDepartureIsNotInTheFutureError>();

        descriptor
            .Field("removeRide")
            .ResolveCompoolerMutation<RemoveRideInput, RemoveRideCommand, Ride>()
            .Error<EntityNotFoundError<Ride, int>>();

        descriptor
            .Field("joinRide")
            .ResolveCompoolerMutation<JoinRideInput, JoinRideCommand, Ride>()
            .Error<EntityNotFoundError<Ride, int>>()
            .Error<EntityNotFoundError<User, string>>()
            .Error<RideErrors.PassengerLimitReachedError>()
            .Error<RideErrors.PassengerIsDriverError>()
            .Error<RideErrors.PassengerAlreadyExistsError>();

        descriptor
            .Field("leaveRide")
            .ResolveCompoolerMutation<LeaveRideInput, LeaveRideCommand, Ride>()
            .Error<EntityNotFoundError<Ride, int>>()
            .Error<EntityNotFoundError<User, string>>()
            .Error<RideErrors.PassengerNotFoundError>();
    }
}
