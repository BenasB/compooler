using Compooler.Application;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Persistence;

public class CompoolerDbContext(DbContextOptions options) : DbContext(options), ICompoolerDbContext
{
    public DbSet<Ride> Rides => Set<Ride>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RidePassenger> RidePassengers => Set<RidePassenger>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RideConfiguration).Assembly);
    }
}
