using Compooler.Persistence.Configurations;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Persistence.DataLoaders.Entities;

public class RideIdsByUserIdDataLoader(
    IServiceProvider serviceProvider,
    IBatchScheduler batchScheduler,
    DataLoaderOptions options
) : CompoolerDbContextGroupedDataLoader<int, int>(serviceProvider, batchScheduler, options)
{
    protected override async Task<ILookup<int, int>> LoadGroupedBatchAsync(
        IReadOnlyList<int> keys,
        CompoolerDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var ridePassengers = await dbContext
            .RidePassengers.Where(cgp => keys.Contains(cgp.UserId))
            .Select(cgp => new
            {
                cgp.UserId,
                RideId = EF.Property<int>(cgp, RideConfiguration.RideIdColumnName)
            })
            .ToListAsync(cancellationToken);

        return ridePassengers.ToLookup(x => x.UserId, x => x.RideId);
    }
}