using Compooler.Domain.Entities.CommuteGroupEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compooler.Persistence.Configurations;

public class CommuteGroupConfiguration
    : IEntityTypeConfiguration<CommuteGroup>,
        IEntityTypeConfiguration<CommuteGroupPassenger>
{
    public const string CommuteGroupIdColumnName = "CommuteGroupId";

    public void Configure(EntityTypeBuilder<CommuteGroup> builder)
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

        builder.HasMany(x => x.Passengers).WithOne().HasForeignKey(CommuteGroupIdColumnName);
    }

    public void Configure(EntityTypeBuilder<CommuteGroupPassenger> builder)
    {
        builder.Property(x => x.JoinedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasKey(CommuteGroupIdColumnName, nameof(CommuteGroupPassenger.UserId));
    }
}