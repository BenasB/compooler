using Compooler.Domain.Entities.UserEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compooler.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public const int IdLength = 28; // Length of the Ids created by Firebase Authentication

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType($"varchar({IdLength})");
    }
}
