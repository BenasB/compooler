using Compooler.Application.Commands;
using Compooler.Domain.Entities.UserEntity;
using JetBrains.Annotations;

namespace Compooler.API.Types.Mutations.Inputs;

[PublicAPI]
public record CreateUserInput(string FirstName, string LastName) : IMappableTo<CreateUserCommand>
{
    public CreateUserCommand Map() => new(FirstName: FirstName, LastName: LastName);
}

[PublicAPI]
public record RemoveUserInput([property: ID<User>] int Id) : IMappableTo<RemoveUserCommand>
{
    public RemoveUserCommand Map() => new(Id: Id);
}
