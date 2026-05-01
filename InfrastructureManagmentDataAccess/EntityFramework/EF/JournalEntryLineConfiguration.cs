using InfrastrfuctureManagmentCore.Domains.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
    {
        public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
        {
            builder.ToTable("AccountingJournalEntryLines");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.DebitAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.CreditAmount)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.FinancialAccount)
                .WithMany()
                .HasForeignKey(x => x.FinancialAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CostCenter)
                .WithMany()
                .HasForeignKey(x => x.CostCenterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
