using Compooler.API.Types.Errors;
using Compooler.API.Types.Mutations;
using Compooler.API.Types.Objects;
using Compooler.API.Types.Queries;
using Compooler.Persistence.DataLoaders.Entities;
using HotChocolate.Execution.Configuration;

namespace Compooler.API.Extensions;

public static class TypeRegistration
{
    public static IRequestExecutorBuilder AddCompoolerTypes(this IRequestExecutorBuilder builder) =>
        builder.AddQueries().AddMutations().AddObjectTypes().AddDataLoaders().AddErrorTypes();

    private static IRequestExecutorBuilder AddQueries(this IRequestExecutorBuilder builder) =>
        builder.AddQueryType<RideQueries>().AddTypeExtension<UserQueries>();

    private static IRequestExecutorBuilder AddMutations(this IRequestExecutorBuilder builder) =>
        builder.AddMutationType<RideMutations>().AddTypeExtension<UserMutations>();

    private static IRequestExecutorBuilder AddObjectTypes(this IRequestExecutorBuilder builder) =>
        builder
            .AddType<RideNodeType>()
            .AddType<RouteObjectType>()
            .AddType<RidePassengerObjectType>()
            .AddType<GeographicCoordinatesObjectType>()
            .AddType<UserNodeType>();

    private static IRequestExecutorBuilder AddDataLoaders(this IRequestExecutorBuilder builder) =>
        builder
            .AddDataLoader<UserByIdDataLoader>()
            .AddDataLoader<RideByIdDataLoader>()
            .AddDataLoader<RideIdsByUserIdDataLoader>();

    private static IRequestExecutorBuilder AddErrorTypes(this IRequestExecutorBuilder builder) =>
        builder
            .AddErrorInterfaceType<ErrorInterfaceType>()
            .TryAddTypeInterceptor<EntityNotFoundErrorTypeInterceptor>()
            .AddTypeExtension<PassengerNotFoundErrorExtension>()
            .AddTypeExtension<PassengerIsDriverErrorExtension>()
            .AddTypeExtension<PassengerAlreadyExistsErrorExtension>();
}
