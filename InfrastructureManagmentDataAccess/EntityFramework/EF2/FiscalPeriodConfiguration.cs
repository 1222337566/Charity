using InfrastrfuctureManagmentCore.Domains.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class FiscalPeriodConfiguration : IEntityTypeConfiguration<FiscalPeriod>
    {
        public void Configure(EntityTypeBuilder<FiscalPeriod> builder)
        {
            builder.ToTable("AccountingFiscalPeriods");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PeriodCode).IsRequired().HasMaxLength(20);
            builder.Property(x => x.PeriodNameAr).IsRequired().HasMaxLength(200);
            builder.Property(x => x.PeriodNameEn).HasMaxLength(200);
            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.Property(x => x.StartDate).HasColumnType("date");
            builder.Property(x => x.EndDate).HasColumnType("date");
            builder.Property(x => x.IsClosed)
    .HasDefaultValue(false);

            builder.Property(x => x.ClosedByUserId)
                .HasMaxLength(450);

            builder.Property(x => x.ClosingNotes)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.IsClosed);

            builder.HasIndex(x => x.PeriodCode).IsUnique();
        }
    }
}
