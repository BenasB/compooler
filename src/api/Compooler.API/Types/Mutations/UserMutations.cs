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
            .ResolveCompoolerMutation<CreateUserInput, CreateUserCommand, User>();

        descriptor
            .Field("removeUser")
            .ResolveCompoolerMutation<RemoveUserInput, RemoveUserCommand, User>()
            .Error<EntityNotFoundError<User, string>>();
    }
}
