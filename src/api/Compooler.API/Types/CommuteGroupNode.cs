using Compooler.API.DataLoaders;
using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Domain.Entities.UserEntity;
using Route = Compooler.Domain.Entities.CommuteGroupEntity.Route;

namespace Compooler.API.Types;

public class CommuteGroupNode : ObjectType<CommuteGroup>
{
    protected override void Configure(IObjectTypeDescriptor<CommuteGroup> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.MaxPassengers);
        descriptor.Field(x => x.Route);
        descriptor.Field(x => x.Passengers);

        descriptor
            .Field("driver")
            .Type<NonNullType<ObjectType<User>>>()
            .Resolve(async ctx =>
            {
                var dataLoader = ctx.Services.GetRequiredService<IUserByIdDataLoader>();
                var group = ctx.Parent<CommuteGroup>();
                return await dataLoader.LoadAsync(group.DriverId, ctx.RequestAborted);
            });
    }
}

public class RouteNode : ObjectType<Route>
{
    protected override void Configure(IObjectTypeDescriptor<Route> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.Start);
        descriptor.Field(x => x.Finish);
    }
}

public class CommuteGroupPassengerNode : ObjectType<CommuteGroupPassenger>
{
    protected override void Configure(IObjectTypeDescriptor<CommuteGroupPassenger> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.JoinedAt);

        descriptor
            .Field("user")
            .Type<NonNullType<ObjectType<User>>>()
            .Resolve(async ctx =>
            {
                var dataLoader = ctx.Services.GetRequiredService<IUserByIdDataLoader>();
                var groupPassenger = ctx.Parent<CommuteGroupPassenger>();
                return await dataLoader.LoadAsync(groupPassenger.UserId, ctx.RequestAborted);
            });
    }
}
