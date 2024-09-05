using Compooler.Application.Commands;
using Compooler.Domain.Entities.UserEntity;
using JetBrains.Annotations;

namespace Compooler.API.Types.Mutations.Inputs;

[PublicAPI]
public record CreateUserInput(string IdpId, string FirstName, string LastName)
    : IMappableTo<CreateUserCommand>
{
    public CreateUserCommand Map() => new(Id: IdpId, FirstName: FirstName, LastName: LastName);
}

[PublicAPI]
public record RemoveUserInput([property: ID<User>] string Id) : IMappableTo<RemoveUserCommand>
{
    public RemoveUserCommand Map() => new(Id: Id);
}
