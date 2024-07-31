using Compooler.Application;
using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Persistence;

public class CompoolerDbContext(DbContextOptions options) : DbContext(options), ICompoolerDbContext
{
    public DbSet<CommuteGroup> CommuteGroups => Set<CommuteGroup>();
    public DbSet<User> Users => Set<User>();
    public DbSet<CommuteGroupPassenger> CommuteGroupsPassengers => Set<CommuteGroupPassenger>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommuteGroupConfiguration).Assembly);
    }
}
