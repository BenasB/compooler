using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence.DataLoaders;
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
