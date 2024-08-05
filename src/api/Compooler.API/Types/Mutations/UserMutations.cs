using Compooler.API.Types.Objects.Inputs;
using Compooler.Application;
using Compooler.Application.Commands;
using Compooler.Domain.Entities.UserEntity;
using JetBrains.Annotations;

namespace Compooler.API.Types.Mutations;

[PublicAPI]
public class UserMutations : ObjectTypeExtension
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Mutation);

        descriptor
            .Field("createUser")
            .Argument("input", x => x.Type<InputObjectType<CreateUserInput>>())
            .Type<NonNullType<ObjectType<User>>>()
            .Resolve<User>(async ctx =>
            {
                var input = ctx.ArgumentValue<CreateUserInput>("input");
                var handler = ctx.Services.GetRequiredService<
                    ICommandHandler<CreateUserCommand, User>
                >();

                var command = new CreateUserCommand(
                    FirstName: input.FirstName,
                    LastName: input.LastName
                );

                var result = await handler.HandleAsync(command);

                return result.Value!;
            });
    }
}
