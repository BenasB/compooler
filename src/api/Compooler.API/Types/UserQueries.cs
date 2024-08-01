using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Compooler.API.Types;

[QueryType]
public static class UserQueries
{
    public static async Task<IReadOnlyList<User>> GetUsersAsync(
        CompoolerDbContext dbContext,
        CancellationToken ct
    )
    {
        return await dbContext.Users.AsNoTracking().ToListAsync(ct);
    }
}
