using Compooler.API.Extensions;
using Compooler.API.Types.Mutations.Inputs;
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
            .Authorize()
            .ResolveCompoolerMutation<CreateUserInput, CreateUserCommand, User>();

        descriptor
            .Field("removeUser")
            .Authorize()
            .ResolveCompoolerMutation<RemoveUserCommand, User>(ctx => new RemoveUserCommand(
                Id: ctx.GetRequiredUserId()
            ))
            .Error<EntityNotFoundError<User, string>>();
    }
}
