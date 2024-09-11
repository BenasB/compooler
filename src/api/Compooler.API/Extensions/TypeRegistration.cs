using Compooler.API.Types.Errors;
using Compooler.API.Types.Mutations;
using Compooler.API.Types.Objects;
using Compooler.API.Types.Queries;
using HotChocolate.Execution.Configuration;

namespace Compooler.API.Extensions;

public static class TypeRegistration
{
    public static IRequestExecutorBuilder AddCompoolerTypes(this IRequestExecutorBuilder builder)
    {
        builder.Services.AddCompoolerDataLoaders();

        return builder.AddQueries().AddMutations().AddObjectTypes().AddErrorTypes();
    }

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

    private static IRequestExecutorBuilder AddErrorTypes(this IRequestExecutorBuilder builder) =>
        builder
            .AddErrorInterfaceType<ErrorInterfaceType>()
            .TryAddTypeInterceptor<EntityNotFoundErrorTypeInterceptor>()
            .TryAddTypeInterceptor<EntityAlreadyExistsErrorTypeInterceptor>()
            .AddTypeExtension<PassengerNotFoundErrorExtension>()
            .AddTypeExtension<PassengerIsDriverErrorExtension>()
            .AddTypeExtension<PassengerAlreadyExistsErrorExtension>();
}
