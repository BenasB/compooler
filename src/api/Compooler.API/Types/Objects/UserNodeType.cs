using Compooler.API.DataLoaders.Entities;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using JetBrains.Annotations;

namespace Compooler.API.Types.Objects;

[PublicAPI]
public class UserNodeType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
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
                    ctx.Services.GetRequiredService<RideIdsByUserIdDataLoader>();
                var rideDataLoaderById = ctx.Services.GetRequiredService<RideByIdDataLoader>();
                var user = ctx.Parent<User>();

                var ridePassengers = await rideIdsByUserIdDataLoader.LoadAsync(
                    user.Id,
                    ctx.RequestAborted
                );

                return await rideDataLoaderById.LoadAsync(ridePassengers, ctx.RequestAborted);
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
