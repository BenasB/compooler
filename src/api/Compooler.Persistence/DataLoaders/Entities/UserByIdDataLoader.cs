using Compooler.Domain.Entities.UserEntity;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Persistence.DataLoaders.Entities;

public class UserByIdDataLoader(
    IServiceProvider serviceProvider,
    IBatchScheduler batchScheduler,
    DataLoaderOptions options
) : CompoolerDbContextBatchDataLoader<string, User>(serviceProvider, batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<string, User>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CompoolerDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        return await dbContext
            .Users.AsNoTracking()
            .Where(u => keys.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);
    }
}
