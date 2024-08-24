using Compooler.API.DataLoaders.Entities;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using JetBrains.Annotations;

namespace Compooler.API.Types.Objects;

[PublicAPI]
public class RidePassengerObjectType : ObjectType<RidePassenger>
{
    protected override void Configure(IObjectTypeDescriptor<RidePassenger> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.JoinedAt);

        descriptor
            .Field("user")
            .Type<NonNullType<ObjectType<User>>>()
            .Resolve<User>(async ctx =>
            {
                var dataLoader = ctx.Services.GetRequiredService<UserByIdDataLoader>();
                var groupPassenger = ctx.Parent<RidePassenger>();
                return await dataLoader.LoadAsync(groupPassenger.UserId, ctx.RequestAborted);
            });
    }
}
