using Compooler.API.Extensions;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence.DataLoaders;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;
using JetBrains.Annotations;

namespace Compooler.API.Types.Objects;

[PublicAPI]
public class UserNodeType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Authorize();
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.FirstName);
        descriptor.Field(x => x.LastName);

        descriptor
            .Field("upcomingRides")
            .UsePaging()
            .Type<NonNullType<ListType<NonNullType<ObjectType<Ride>>>>>()
            .Resolve<Connection<Ride>>(async ctx =>
            {
                var upcomingRideIdsByUserIdDataLoader =
                    ctx.Services.GetRequiredService<IUpcomingRideIdsByUserIdDataLoader>();
                var rideDataLoaderById = ctx.Services.GetRequiredService<IRideByIdDataLoader>();
                var user = ctx.Parent<User>();
                var pagingArguments = ctx.GetPagingArguments();

                var idsPage = await upcomingRideIdsByUserIdDataLoader
                    .WithPagingArguments(pagingArguments)
                    .LoadAsync(user.Id, ctx.RequestAborted);

                return await RideIdsPageToRideConnection(
                    rideDataLoaderById,
                    idsPage,
                    ctx.RequestAborted
                );
            });

        descriptor
            .Field("historicalRides")
            .UsePaging()
            .Type<NonNullType<ListType<NonNullType<ObjectType<Ride>>>>>()
            .Resolve<Connection<Ride>>(async ctx =>
            {
                var historicalRideIdsByUserIdDataLoader =
                    ctx.Services.GetRequiredService<IHistoricalRideIdsByUserIdDataLoader>();
                var rideDataLoaderById = ctx.Services.GetRequiredService<IRideByIdDataLoader>();
                var user = ctx.Parent<User>();
                var pagingArguments = ctx.GetPagingArguments();

                var idsPage = await historicalRideIdsByUserIdDataLoader
                    .WithPagingArguments(pagingArguments)
                    .LoadAsync(user.Id, ctx.RequestAborted);

                return await RideIdsPageToRideConnection(
                    rideDataLoaderById,
                    idsPage,
                    ctx.RequestAborted
                );
            });

        descriptor
            .ImplementsNode()
            .IdField(x => x.Id)
            .ResolveNode(
                async (ctx, id) =>
                {
                    var dataLoader = ctx.Services.GetRequiredService<UserByIdDataLoader>();
                    return await dataLoader.LoadAsync(id, ctx.RequestAborted);
                }
            );
    }

    private static async Task<Connection<Ride>> RideIdsPageToRideConnection(
        IRideByIdDataLoader dataLoader,
        Page<(DateTimeOffset timeOfDeparture, int RideId)> idsPage,
        CancellationToken cancellationToken
    )
    {
        if (idsPage.Items == null)
            return Connection.Empty<Ride>();

        var rides = await dataLoader.LoadRequiredAsync(
            idsPage.Items.Select(x => x.RideId).ToList(),
            cancellationToken
        );

        return new Page<Ride>(
            items: [.. rides],
            hasNextPage: idsPage.HasNextPage,
            hasPreviousPage: idsPage.HasPreviousPage,
            createCursor: ride => idsPage.CreateCursor((ride.TimeOfDeparture, ride.Id))
        ).ToConnection();
    }
}
