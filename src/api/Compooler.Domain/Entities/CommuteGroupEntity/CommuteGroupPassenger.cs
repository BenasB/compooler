namespace Compooler.Domain.Entities.CommuteGroupEntity;

public sealed class CommuteGroupPassenger
{
    public required int UserId { get; init; }
    public DateTimeOffset JoinedAt { get; }

    private CommuteGroupPassenger() { }

    public static CommuteGroupPassenger Create(int userId) => new() { UserId = userId };
}
