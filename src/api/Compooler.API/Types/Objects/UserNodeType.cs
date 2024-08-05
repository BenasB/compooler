using Compooler.API.DataLoaders.Entities;
using Compooler.Domain.Entities.CommuteGroupEntity;
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
            .Field("commuteGroups")
            .Type<NonNullType<ListType<NonNullType<ObjectType<CommuteGroup>>>>>()
            .Resolve<IReadOnlyList<CommuteGroup>>(async ctx =>
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
