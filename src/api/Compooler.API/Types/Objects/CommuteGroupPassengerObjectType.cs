using Compooler.API.DataLoaders;
using Compooler.API.DataLoaders.Entities;
using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Domain.Entities.UserEntity;

namespace Compooler.API.Types.Objects;

public class CommuteGroupPassengerObjectType : ObjectType<CommuteGroupPassenger>
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
                var dataLoader = ctx.Services.GetRequiredService<UserByIdDataLoader>();
                var groupPassenger = ctx.Parent<CommuteGroupPassenger>();
                return await dataLoader.LoadAsync(groupPassenger.UserId, ctx.RequestAborted);
            });
    }
}
