using InfrastrfuctureManagmentCore.Domains.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
    {
        public void Configure(EntityTypeBuilder<JournalEntry> builder)
        {
            builder.ToTable("AccountingJournalEntries");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EntryNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Notes)
                .HasMaxLength(2000);

            builder.Property(x => x.SourceType)
                .HasMaxLength(100);

            builder.Property(x => x.TotalDebit)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TotalCredit)
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.EntryNumber)
                .IsUnique();

            builder.HasOne(x => x.FiscalPeriod)
                .WithMany()
                .HasForeignKey(x => x.FiscalPeriodId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Lines)
                .WithOne(x => x.JournalEntry)
                .HasForeignKey(x => x.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
