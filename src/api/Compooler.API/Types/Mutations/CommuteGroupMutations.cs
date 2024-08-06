using Compooler.API.Types.Mutations.Inputs;
using Compooler.Application;
using Compooler.Application.Commands;
using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Domain.Entities.UserEntity;
using JetBrains.Annotations;

namespace Compooler.API.Types.Mutations;

[PublicAPI]
public class CommuteGroupMutations : ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Mutation);

        descriptor
            .Field("createCommuteGroup")
            .ResolveCompoolerMutation<
                CreateCommuteGroupInput,
                CreateCommuteGroupCommand,
                CommuteGroup,
                GeographicCoordinatesErrors.InvalidLatitudeError,
                GeographicCoordinatesErrors.InvalidLongitudeError,
                EntityNotFoundError<User>
            >(input => new CreateCommuteGroupCommand(
                DriverId: input.DriverId,
                MaxPassengers: input.MaxPassengers,
                StartLatitude: input.StartLatitude,
                StartLongitude: input.StartLongitude,
                FinishLatitude: input.FinishLatitude,
                FinishLongitude: input.FinishLongitude
            ));
    }
}
