using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class GrantInstallmentConfiguration : IEntityTypeConfiguration<GrantInstallment>
    {
        public void Configure(EntityTypeBuilder<GrantInstallment> builder)
        {
            builder.ToTable("CharityGrantInstallments");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.ReceivedAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.Property(x => x.ReferenceNumber).HasMaxLength(100);
            builder.Property(x => x.Notes).HasMaxLength(4000);

            builder.HasOne(x => x.GrantAgreement)
                .WithMany(x => x.Installments)
                .HasForeignKey(x => x.GrantAgreementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.PaymentMethod)
                .WithMany()
                .HasForeignKey(x => x.PaymentMethodId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.FinancialAccount)
                .WithMany()
                .HasForeignKey(x => x.FinancialAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => new { x.GrantAgreementId, x.InstallmentNumber }).IsUnique();
            builder.HasIndex(x => new { x.DueDate, x.Status });
        }
    }
}
