using Compooler.Domain.Entities.RideEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compooler.Persistence.Configurations;

public class RideConfiguration
    : IEntityTypeConfiguration<Ride>,
        IEntityTypeConfiguration<RidePassenger>
{
    public const string RideIdColumnName = "RideId";

    public void Configure(EntityTypeBuilder<Ride> builder)
    {
        builder.HasKey(x => x.Id);

        builder.OwnsOne(
            x => x.Route,
            routeBuilder =>
            {
                routeBuilder.ToTable("Routes");
                routeBuilder.OwnsOne(route => route.Start);
                routeBuilder.OwnsOne(route => route.Finish);
            }
        );

        builder.Navigation(x => x.Passengers).AutoInclude();

        builder.HasMany(x => x.Passengers).WithOne().HasForeignKey(RideIdColumnName);
    }

    public void Configure(EntityTypeBuilder<RidePassenger> builder)
    {
        builder.Property(x => x.JoinedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasKey(RideIdColumnName, nameof(RidePassenger.UserId));
    }
}