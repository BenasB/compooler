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
            .Field("rides")
            .Type<NonNullType<ListType<NonNullType<ObjectType<Ride>>>>>()
            .Resolve<IReadOnlyList<Ride>>(async ctx =>
            {
                var rideIdsByUserIdDataLoader =
                    ctx.Services.GetRequiredService<IRideIdsByUserIdDataLoader>();
                var rideDataLoaderById = ctx.Services.GetRequiredService<IRideByIdDataLoader>();
                var user = ctx.Parent<User>();

                var ridePassengers = await rideIdsByUserIdDataLoader.LoadRequiredAsync(
                    user.Id,
                    ctx.RequestAborted
                );

                return await rideDataLoaderById.LoadRequiredAsync(
                    ridePassengers,
                    ctx.RequestAborted
                );
            });

        descriptor
            .Field("testRides")
            .UsePaging()
            .Type<NonNullType<ListType<NonNullType<ObjectType<Ride>>>>>()
            .Resolve<Connection<Ride>>(async ctx =>
            {
                var rideIdsByUserIdDataLoader =
                    ctx.Services.GetRequiredService<ITestRideIdsByUserIdDataLoader>();
                var rideDataLoaderById = ctx.Services.GetRequiredService<IRideByIdDataLoader>();
                var user = ctx.Parent<User>();
                var pagingArguments = ctx.GetPagingArguments();

                var idsPage = await rideIdsByUserIdDataLoader
                    .WithPagingArguments(pagingArguments)
                    .LoadAsync(user.Id, ctx.RequestAborted);

                if (idsPage.Items == null)
                    return Connection.Empty<Ride>();

                var rides = await rideDataLoaderById.LoadRequiredAsync(
                    idsPage.Items.Select(x => x.RideId).ToList(),
                    ctx.RequestAborted
                );

                return new Page<Ride>(
                    items: [.. rides],
                    hasNextPage: idsPage.HasNextPage,
                    hasPreviousPage: idsPage.HasPreviousPage,
                    createCursor: ride => idsPage.CreateCursor((ride.TimeOfDeparture, ride.Id))
                ).ToConnection();
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
}
