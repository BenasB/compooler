using Compooler.API.DataLoaders;
using Compooler.API.DataLoaders.Entities;
using Compooler.API.Types;
using Compooler.API.Types.Objects;
using Compooler.API.Types.Queries;
using HotChocolate.Execution.Configuration;

namespace Compooler.API.Extensions;

internal static class TypeRegistration
{
    internal static IRequestExecutorBuilder AddCompoolerTypes(
        this IRequestExecutorBuilder builder
    ) => builder.AddQueries().AddObjectTypes().AddDataLoaders();

    private static IRequestExecutorBuilder AddQueries(this IRequestExecutorBuilder builder) =>
        builder.AddQueryType<CommuteGroupQueries>().AddTypeExtension<UserQueries>();

    private static IRequestExecutorBuilder AddObjectTypes(this IRequestExecutorBuilder builder) =>
        builder
            .AddType<CommuteGroupNodeType>()
            .AddType<RouteObjectType>()
            .AddType<CommuteGroupPassengerObjectType>()
            .AddType<UserNodeType>();

    private static IRequestExecutorBuilder AddDataLoaders(this IRequestExecutorBuilder builder) =>
        builder
            .AddDataLoader<UserByIdDataLoader>()
            .AddDataLoader<CommuteGroupByIdDataLoader>()
            .AddDataLoader<CommuteGroupIdsByUserIdDataLoader>();
}
