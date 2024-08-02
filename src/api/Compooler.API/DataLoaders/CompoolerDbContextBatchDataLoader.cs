using Compooler.Persistence;

namespace Compooler.API.DataLoaders;

public abstract class CompoolerDbContextBatchDataLoader<TKey, TValue>(
    IServiceProvider serviceProvider,
    IBatchScheduler batchScheduler,
    DataLoaderOptions options
) : BatchDataLoader<TKey, TValue>(batchScheduler, options)
    where TKey : notnull
{
    protected override async Task<IReadOnlyDictionary<TKey, TValue>> LoadBatchAsync(
        IReadOnlyList<TKey> keys,
        CancellationToken cancellationToken
    )
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<CompoolerDbContext>();

        return await LoadBatchAsync(keys, dbContext, cancellationToken);
    }

    protected abstract Task<IReadOnlyDictionary<TKey, TValue>> LoadBatchAsync(
        IReadOnlyList<TKey> keys,
        CompoolerDbContext dbContext,
        CancellationToken cancellationToken
    );
}
