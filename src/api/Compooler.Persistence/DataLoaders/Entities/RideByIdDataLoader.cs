using Compooler.Domain.Entities.RideEntity;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Persistence.DataLoaders.Entities;

public class RideByIdDataLoader(
    IServiceProvider serviceProvider,
    IBatchScheduler batchScheduler,
    DataLoaderOptions options
) : CompoolerDbContextBatchDataLoader<int, Ride>(serviceProvider, batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<int, Ride>> LoadBatchAsync(
        IReadOnlyList<int> keys,
        CompoolerDbContext dbContext,
        CancellationToken cancellationToken
    ) =>
        await dbContext
            .Rides.AsNoTracking()
            .Where(cg => keys.Contains(cg.Id))
            .ToDictionaryAsync(cg => cg.Id, cancellationToken);
}
