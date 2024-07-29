namespace Compooler.Domain.Entities.UserEntity;

public sealed class User
{
    public required int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}
