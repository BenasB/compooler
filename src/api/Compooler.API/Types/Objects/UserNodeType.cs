using Compooler.API.DataLoaders;
using Compooler.API.DataLoaders.Entities;
using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Domain.Entities.UserEntity;

namespace Compooler.API.Types.Objects;

public class UserNodeType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.FirstName);
        descriptor.Field(x => x.LastName);

        descriptor
            .Field("commuteGroups")
            .Type<NonNullType<ListType<NonNullType<ObjectType<CommuteGroup>>>>>()
            .Resolve(async ctx =>
            {
                var commuteGroupIdsByUserIdDataLoader =
                    ctx.Services.GetRequiredService<CommuteGroupIdsByUserIdDataLoader>();
                var commuteGroupDataLoaderById =
                    ctx.Services.GetRequiredService<CommuteGroupByIdDataLoader>();
                var user = ctx.Parent<User>();

                var commuteGroupPassengers = await commuteGroupIdsByUserIdDataLoader.LoadAsync(
                    user.Id,
                    ctx.RequestAborted
                );

                return await commuteGroupDataLoaderById.LoadAsync(
                    commuteGroupPassengers,
                    ctx.RequestAborted
                );
            });
    }
}
