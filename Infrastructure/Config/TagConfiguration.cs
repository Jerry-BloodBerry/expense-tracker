using Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
  public void Configure(EntityTypeBuilder<Tag> builder)
  {
    builder.HasKey(t => t.Id);

    builder.Property(t => t.Name)
        .IsRequired()
        .HasMaxLength(50);

    builder.HasIndex(t => t.Name)
        .IsUnique();
  }
}