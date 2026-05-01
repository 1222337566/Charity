using InfrastrfuctureManagmentCore.Domains.Payroll;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class PayrollPaymentConfiguration : IEntityTypeConfiguration<PayrollPayment>
    {
        public void Configure(EntityTypeBuilder<PayrollPayment> builder)
        {
            builder.ToTable("CharityPayrollPayments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.ReferenceNumber).HasMaxLength(200);
            builder.Property(x => x.Notes).HasMaxLength(2000);

            builder.HasOne(x => x.PayrollEmployee)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.PayrollEmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.PaymentMethod)
                .WithMany()
                .HasForeignKey(x => x.PaymentMethodId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.FinancialAccount)
                .WithMany()
                .HasForeignKey(x => x.FinancialAccountId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
