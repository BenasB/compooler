using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Persistence;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Compooler.API.DataLoaders.Entities;

[UsedImplicitly]
public class CommuteGroupByIdDataLoader(
    IServiceProvider serviceProvider,
    IBatchScheduler batchScheduler,
    DataLoaderOptions options
) : CompoolerDbContextBatchDataLoader<int, CommuteGroup>(serviceProvider, batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<int, CommuteGroup>> LoadBatchAsync(
        IReadOnlyList<int> keys,
        CompoolerDbContext dbContext,
        CancellationToken cancellationToken
    ) =>
        await dbContext
            .CommuteGroups.Where(cg => keys.Contains(cg.Id))
            .ToDictionaryAsync(cg => cg.Id, cancellationToken);
}
