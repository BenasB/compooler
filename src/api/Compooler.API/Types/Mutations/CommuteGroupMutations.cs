using Compooler.Application;
using Compooler.Application.Commands;
using Compooler.Domain.Entities.CommuteGroupEntity;
using JetBrains.Annotations;

namespace Compooler.API.Types.Mutations;

[PublicAPI]
public class CommuteGroupMutations : ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Mutation);

        descriptor
            .Field("createCommuteGroup")
            .Argument("input", x => x.Type<InputObjectType<CreateCommuteGroupInput>>())
            .Type<NonNullType<ObjectType<CommuteGroup>>>()
            .Resolve<CommuteGroup>(async ctx =>
            {
                var handler = ctx.Services.GetRequiredService<
                    ICommandHandler<CreateCommuteGroupCommand, CommuteGroup>
                >();

                var result = await handler.HandleAsync(new CreateCommuteGroupCommand());

                return result.Value!;
            });
    }
}

[PublicAPI]
public record CreateCommuteGroupInput(int Id, string Name);
