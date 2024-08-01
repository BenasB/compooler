using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Persistence;
using Compooler.Persistence.Configurations;
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

    [DataLoader]
    internal static async Task<ILookup<int, int>> GetCommuteGroupIdsByUserIdAsync(
        IReadOnlyList<int> keys,
        CompoolerDbContext dbContext,
        CancellationToken ct
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
            .ToListAsync(ct);

        return commuteGroupPassengers.ToLookup(x => x.UserId, x => x.CommuteGroupId);
    }
}
