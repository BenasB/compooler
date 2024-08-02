using Compooler.Persistence;
using Compooler.Persistence.Configurations;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Compooler.API.DataLoaders.Entities;

[UsedImplicitly]
public class CommuteGroupIdsByUserIdDataLoader(
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
        var commuteGroupPassengers = await dbContext
            .CommuteGroupsPassengers.Where(cgp => keys.Contains(cgp.UserId))
            .Select(cgp => new
            {
                cgp.UserId,
                CommuteGroupId = EF.Property<int>(
                    cgp,
                    CommuteGroupConfiguration.CommuteGroupIdColumnName
                )
            })
            .ToListAsync(cancellationToken);

        return commuteGroupPassengers.ToLookup(x => x.UserId, x => x.CommuteGroupId);
    }
}
