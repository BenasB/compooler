using Compooler.Domain.Entities.UserEntity;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Persistence.DataLoaders;

public sealed class UserDataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<string, User>> GetUserByIdAsync(
        IReadOnlyList<string> keys,
        CompoolerDbContext dbContext,
        CancellationToken cancellationToken
    ) =>
        await dbContext
            .Users.AsNoTracking()
            .Where(u => keys.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);
}
