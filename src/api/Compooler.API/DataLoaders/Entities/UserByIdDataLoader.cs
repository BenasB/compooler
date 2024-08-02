using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Compooler.API.DataLoaders.Entities;

[UsedImplicitly]
public class UserByIdDataLoader(
    IServiceProvider serviceProvider,
    IBatchScheduler batchScheduler,
    DataLoaderOptions options
) : CompoolerDbContextBatchDataLoader<int, User>(serviceProvider, batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<int, User>> LoadBatchAsync(
        IReadOnlyList<int> keys,
        CompoolerDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        return await dbContext
            .Users.Where(u => keys.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);
    }
}
