using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Compooler.API.DataLoaders;

internal static class UserDataLoader
{
    [DataLoader]
    internal static async Task<IReadOnlyDictionary<int, User>> GetUserByIdAsync(
        IReadOnlyList<int> keys,
        CompoolerDbContext dbContext,
        CancellationToken ct
    ) => await dbContext.Users.Where(u => keys.Contains(u.Id)).ToDictionaryAsync(u => u.Id, ct);
}
