using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using JetBrains.Annotations;

namespace Compooler.API.Types.Errors;

[UsedImplicitly]
public class PassengerNotFoundErrorExtension
    : ObjectTypeExtension<RideErrors.PassengerNotFoundError>
{
    protected override void Configure(
        IObjectTypeDescriptor<RideErrors.PassengerNotFoundError> descriptor
    )
    {
        descriptor.Field(x => x.UserId).ID(nameof(User));
    }
}

[UsedImplicitly]
public class PassengerIsDriverErrorExtension
    : ObjectTypeExtension<RideErrors.PassengerIsDriverError>
{
    protected override void Configure(
        IObjectTypeDescriptor<RideErrors.PassengerIsDriverError> descriptor
    )
    {
        descriptor.Field(x => x.DriverId).ID(nameof(User));
    }
}

[UsedImplicitly]
public class PassengerAlreadyExistsErrorExtension
    : ObjectTypeExtension<RideErrors.PassengerAlreadyExistsError>
{
    protected override void Configure(
        IObjectTypeDescriptor<RideErrors.PassengerAlreadyExistsError> descriptor
    )
    {
        descriptor.Field(x => x.PassengerId).ID(nameof(User));
    }
}
