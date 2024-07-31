using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Compooler.API.Types;

[QueryType]
public static class CommuteGroupQueries
{
    public static async Task<IReadOnlyList<CommuteGroup>> GetCommuteGroupsAsync(
        CompoolerDbContext dbContext,
        CancellationToken ct
    )
    {
        return await dbContext.CommuteGroups.AsNoTracking().ToListAsync(ct);
    }
}
