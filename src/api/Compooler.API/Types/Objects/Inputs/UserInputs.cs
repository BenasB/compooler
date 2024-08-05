using JetBrains.Annotations;

namespace Compooler.API.Types.Objects.Inputs;

[PublicAPI]
public record CreateUserInput(string FirstName, string LastName);

[PublicAPI]
public record DeleteUserInput(int Id);
