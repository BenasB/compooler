namespace Compooler.Domain.Entities.UserEntity;

public sealed class User : IEntity
{
    public int Id { get; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    private User() { }

    public static User Create(string firstName, string lastName) =>
        new() { FirstName = firstName, LastName = lastName };
}
