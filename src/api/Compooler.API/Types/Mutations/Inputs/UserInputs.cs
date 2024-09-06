using Compooler.API.Extensions;
using Compooler.Application.Commands;
using Compooler.Domain.Entities.UserEntity;
using HotChocolate.Resolvers;
using JetBrains.Annotations;

namespace Compooler.API.Types.Mutations.Inputs;

[PublicAPI]
public record CreateUserInput(string FirstName, string LastName) : IMappableTo<CreateUserCommand>
{
    public CreateUserCommand Map(IResolverContext ctx) =>
        new(Id: ctx.GetRequiredUserId(), FirstName: FirstName, LastName: LastName);
}

[PublicAPI]
public record RemoveUserInput([property: ID<User>] string Id) : IMappableTo<RemoveUserCommand>
{
    public RemoveUserCommand Map(IResolverContext ctx) => new(Id: Id);
}
