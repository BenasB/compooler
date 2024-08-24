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
);
