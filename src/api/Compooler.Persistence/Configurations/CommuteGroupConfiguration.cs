using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Domain.Entities.UserEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compooler.Persistence.Configurations;

public class CommuteGroupConfiguration
    : IEntityTypeConfiguration<CommuteGroup>,
        IEntityTypeConfiguration<CommuteGroupPassenger>
{
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
    }

    public void Configure(EntityTypeBuilder<CommuteGroupPassenger> builder)
    {
        builder.Property(x => x.JoinedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        const string groupIdColumn = "CommuteGroupId";
        builder.HasKey(groupIdColumn, nameof(CommuteGroupPassenger.UserId));
    }
}
