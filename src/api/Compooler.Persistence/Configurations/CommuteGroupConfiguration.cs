using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Domain.Entities.UserEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compooler.Persistence.Configurations;

public class CommuteGroupConfiguration : IEntityTypeConfiguration<CommuteGroup>
{
    public void Configure(EntityTypeBuilder<CommuteGroup> builder)
    {
        builder.OwnsOne(
            x => x.Route,
            routeBuilder =>
            {
                routeBuilder.ToTable("Routes");
                routeBuilder.OwnsOne(route => route.Start);
                routeBuilder.OwnsOne(route => route.Finish);
            }
        );

        builder
            .HasOne<User>()
            .WithMany()
            .HasPrincipalKey(user => user.Id)
            .HasForeignKey(group => group.DriverId);

        builder.OwnsMany(
            x => x.Passengers,
            passengersBuilder =>
            {
                passengersBuilder
                    .Property<DateTimeOffset>("JoinedAt")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                const string ownerId = "CommuteGroupId";
                passengersBuilder.Property(p => p.UserId).ValueGeneratedNever();
                passengersBuilder.HasKey(ownerId, nameof(GroupPassenger.UserId));
                passengersBuilder.WithOwner().HasForeignKey(ownerId);
            }
        );
    }
}
