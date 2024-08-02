using Compooler.API.DataLoaders.Entities;
using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Domain.Entities.UserEntity;
using JetBrains.Annotations;

namespace Compooler.API.Types.Objects;

[PublicAPI]
public class CommuteGroupNodeType : ObjectType<CommuteGroup>
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
                var dataLoader = ctx.Services.GetRequiredService<UserByIdDataLoader>();
                var commuteGroup = ctx.Parent<CommuteGroup>();

                return await dataLoader.LoadAsync(commuteGroup.DriverId, ctx.RequestAborted);
            });
    }
}
