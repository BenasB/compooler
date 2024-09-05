namespace Compooler.Domain.Entities.RideEntity;

public sealed class RidePassenger
{
    public required string UserId { get; init; }
    public DateTimeOffset JoinedAt { get; }

    private RidePassenger() { }

    public static RidePassenger Create(string userId) => new() { UserId = userId };
}
