using Compooler.Persistence;

namespace Compooler.API.DataLoaders;

public abstract class CompoolerDbContextGroupedDataLoader<TKey, TValue>(
    IServiceProvider serviceProvider,
    IBatchScheduler batchScheduler,
    DataLoaderOptions options
) : GroupedDataLoader<TKey, TValue>(batchScheduler, options)
    where TKey : notnull
{
    protected override async Task<ILookup<TKey, TValue>> LoadGroupedBatchAsync(
        IReadOnlyList<TKey> keys,
        CancellationToken cancellationToken
    )
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<CompoolerDbContext>();

        return await LoadGroupedBatchAsync(keys, dbContext, cancellationToken);
    }

    protected abstract Task<ILookup<TKey, TValue>> LoadGroupedBatchAsync(
        IReadOnlyList<TKey> keys,
        CompoolerDbContext dbContext,
        CancellationToken cancellationToken
    );
}
