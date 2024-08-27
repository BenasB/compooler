namespace Compooler.Domain.Entities.RideEntity;

public sealed class RidePassenger
{
    public required int UserId { get; init; }
    public DateTimeOffset JoinedAt { get; }

    private RidePassenger() { }

    public static RidePassenger Create(int userId) => new() { UserId = userId };
}
