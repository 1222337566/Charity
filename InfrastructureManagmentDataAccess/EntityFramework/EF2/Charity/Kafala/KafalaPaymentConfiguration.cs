using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Kafala
{
    public class KafalaPaymentConfiguration : IEntityTypeConfiguration<KafalaPayment>
    {
        public void Configure(EntityTypeBuilder<KafalaPayment> builder)
        {
            builder.ToTable("CharityKafalaPayments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Direction).HasMaxLength(30).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(30).IsRequired();
            builder.Property(x => x.PeriodLabel).HasMaxLength(50).IsRequired();
            builder.Property(x => x.ReferenceNumber).HasMaxLength(100);

            builder.HasOne(x => x.KafalaCase).WithMany(x => x.Payments).HasForeignKey(x => x.KafalaCaseId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Sponsor).WithMany(x => x.Payments).HasForeignKey(x => x.SponsorId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.PaymentMethod).WithMany().HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.FinancialAccount).WithMany().HasForeignKey(x => x.FinancialAccountId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
