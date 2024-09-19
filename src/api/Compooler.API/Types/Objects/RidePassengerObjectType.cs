using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence.DataLoaders;
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
                var dataLoader = ctx.Services.GetRequiredService<IUserByIdDataLoader>();
                var groupPassenger = ctx.Parent<RidePassenger>();
                return await dataLoader.LoadRequiredAsync(
                    groupPassenger.UserId,
                    ctx.RequestAborted
                );
            });
    }
}
