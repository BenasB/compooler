using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Compooler.API.DataLoaders;

internal static class CommuteGroupDataLoader
{
    [DataLoader]
    internal static async Task<IReadOnlyDictionary<int, CommuteGroup>> GetCommuteGroupByIdAsync(
        IReadOnlyList<int> keys,
        CompoolerDbContext dbContext,
        CancellationToken ct
    ) =>
        await dbContext
            .CommuteGroups.Where(cg => keys.Contains(cg.Id))
            .ToDictionaryAsync(cg => cg.Id, ct);
}
