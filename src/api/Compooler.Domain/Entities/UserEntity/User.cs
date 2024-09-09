namespace Compooler.Domain.Entities.UserEntity;

public sealed class User : IEntity<string>
{
    public required string Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    private User() { }

    public static User Create(string id, string firstName, string lastName) =>
        new()
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName
        };
}
