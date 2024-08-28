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
            .ResolveCompoolerMutation<
                CreateRideInput,
                CreateRideCommand,
                Ride,
                GeographicCoordinatesErrors.InvalidLatitudeError,
                GeographicCoordinatesErrors.InvalidLongitudeError,
                EntityNotFoundError<User>
            >();

        descriptor
            .Field("removeRide")
            .ResolveCompoolerMutation<
                RemoveRideInput,
                RemoveRideCommand,
                Ride,
                EntityNotFoundError<Ride>
            >();
    }
}
