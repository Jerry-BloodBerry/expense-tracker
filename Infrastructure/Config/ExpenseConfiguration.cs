using Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Config
{
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(e => e.Date)
                .IsRequired();

            builder.Property(e => e.Currency)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(e => e.Description)
                .HasMaxLength(1000);

            builder.Property(e => e.IsRecurring)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.RecurrenceInterval)
                .HasConversion(new EnumToStringConverter<RecurrenceInterval>());

            builder.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey("CategoryId")
                .IsRequired();

            builder.HasMany(e => e.Tags)
                .WithMany()
                .UsingEntity(j => j.ToTable("ExpenseTags"));

            builder.Navigation(e => e.Category).AutoInclude();
            builder.Navigation(e => e.Tags).AutoInclude();

            // Indexes
            builder.HasIndex("CategoryId");
            builder.HasIndex(e => e.Date);
            builder.HasIndex(e => e.IsRecurring);
        }
    }
}