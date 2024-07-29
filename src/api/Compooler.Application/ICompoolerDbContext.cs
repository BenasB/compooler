using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Domain.Entities.UserEntity;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Application;

public interface ICompoolerDbContext
{
    public DbSet<CommuteGroup> CommuteGroups { get; }
    public DbSet<User> Users { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
