using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Persistence;
using Compooler.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Compooler.API.DataLoaders;

internal static class CommuteGroupPassengerDataLoader
{
    [DataLoader]
    internal static async Task<
        ILookup<int, (CommuteGroupPassenger CommuteGroupPassenger, int CommuteGroupId)>
    > GetCommuteGroupPassengersByUserIdAsync(
        IReadOnlyList<int> keys,
        CompoolerDbContext dbContext,
        CancellationToken ct
    )
    {
        var commuteGroupPassengers = await dbContext
            .CommuteGroupsPassengers.Where(cgp => keys.Contains(cgp.UserId))
            .Select(cgp => new
            {
                CommuteGroupPassenger = cgp,
                CommuteGroupId = EF.Property<int>(
                    cgp,
                    CommuteGroupConfiguration.CommuteGroupIdColumnName
                )
            })
            .ToListAsync(ct);

        return commuteGroupPassengers
            .Select(x => (x.CommuteGroupPassenger, x.CommuteGroupId))
            .ToLookup(x => x.CommuteGroupPassenger.UserId);
    }
}
