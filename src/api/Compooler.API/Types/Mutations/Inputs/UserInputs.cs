using Compooler.API.Extensions;
using Compooler.Application.Commands;
using HotChocolate.Resolvers;
using JetBrains.Annotations;

namespace Compooler.API.Types.Mutations.Inputs;

[PublicAPI]
public record CreateUserInput(string FirstName, string LastName) : IMappableTo<CreateUserCommand>
{
    public CreateUserCommand Map(IResolverContext ctx) =>
        new(Id: ctx.GetRequiredUserId(), FirstName: FirstName, LastName: LastName);
}
