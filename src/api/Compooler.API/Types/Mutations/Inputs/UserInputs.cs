using Compooler.Domain.Entities.UserEntity;
using JetBrains.Annotations;

namespace Compooler.API.Types.Mutations.Inputs;

[PublicAPI]
public record CreateUserInput(string FirstName, string LastName);

[PublicAPI]
public record RemoveUserInput([property: ID<User>] int Id);
