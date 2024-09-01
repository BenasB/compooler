using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence.DataLoaders.Entities;
using JetBrains.Annotations;

namespace Compooler.API.Types.Objects;

[PublicAPI]
public class RideNodeType : ObjectType<Ride>
{
    protected override void Configure(IObjectTypeDescriptor<Ride> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.MaxPassengers);
        descriptor.Field(x => x.Route);
        descriptor.Field(x => x.Passengers);
        descriptor.Field(x => x.TimeOfDeparture);

        descriptor
            .Field("driver")
            .Type<NonNullType<ObjectType<User>>>()
            .Resolve<User>(async ctx =>
            {
                var dataLoader = ctx.Services.GetRequiredService<UserByIdDataLoader>();
                var ride = ctx.Parent<Ride>();

                return await dataLoader.LoadRequiredAsync(ride.DriverId, ctx.RequestAborted);
            });

        descriptor
            .ImplementsNode()
            .IdField(x => x.Id)
            .ResolveNode(
                async (ctx, id) =>
                {
                    var dataLoader = ctx.Services.GetRequiredService<RideByIdDataLoader>();
                    return await dataLoader.LoadAsync(id, ctx.RequestAborted);
                }
            );
    }
}
