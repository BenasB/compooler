using Compooler.Persistence.Configurations;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Persistence.DataLoaders.Entities;

public class RideIdsByUserIdDataLoader(
    IServiceProvider serviceProvider,
    IBatchScheduler batchScheduler,
    DataLoaderOptions options
) : CompoolerDbContextGroupedDataLoader<string, int>(serviceProvider, batchScheduler, options)
{
    protected override async Task<ILookup<string, int>> LoadGroupedBatchAsync(
        IReadOnlyList<string> keys,
        CompoolerDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var ridePassengers = await dbContext
            .RidePassengers.AsNoTracking()
            .Where(cgp => keys.Contains(cgp.UserId))
            .Select(cgp => new
            {
                cgp.UserId,
                RideId = EF.Property<int>(cgp, RideConfiguration.RideIdColumnName)
            })
            .ToListAsync(cancellationToken);

        return ridePassengers.ToLookup(x => x.UserId, x => x.RideId);
    }
}
