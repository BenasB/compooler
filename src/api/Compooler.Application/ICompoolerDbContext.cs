using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Application;

public interface ICompoolerDbContext
{
    public DbSet<Ride> Rides { get; }
    public DbSet<User> Users { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
