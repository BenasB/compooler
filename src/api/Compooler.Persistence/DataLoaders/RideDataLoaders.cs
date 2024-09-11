using Compooler.Domain.Entities.RideEntity;
using Compooler.Persistence.Configurations;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Persistence.DataLoaders;

public sealed class RideDataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Ride>> GetRideByIdAsync(
        IReadOnlyList<int> keys,
        CompoolerDbContext dbContext,
        CancellationToken cancellationToken
    ) =>
        await dbContext
            .Rides.AsNoTracking()
            .Where(cg => keys.Contains(cg.Id))
            .ToDictionaryAsync(cg => cg.Id, cancellationToken);

    [DataLoader]
    public static async Task<ILookup<string, int>> GetRideIdsByUserIdAsync(
        IReadOnlyList<string> keys,
        CompoolerDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var ridePassengers = await dbContext
            .RidePassengers.AsNoTracking()
            .Where(rp => keys.Contains(rp.UserId))
            .Select(rp => new
            {
                rp.UserId,
                RideId = EF.Property<int>(rp, RideConfiguration.RideIdColumnName)
            })
            .ToListAsync(cancellationToken);

        return ridePassengers.ToLookup(x => x.UserId, x => x.RideId);
    }
}
