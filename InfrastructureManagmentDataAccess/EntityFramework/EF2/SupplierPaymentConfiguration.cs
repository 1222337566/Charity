using InfrastrfuctureManagmentCore.Domains.Suppliers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SupplierPaymentConfiguration : IEntityTypeConfiguration<SupplierPayment>
{
    public void Configure(EntityTypeBuilder<SupplierPayment> b)
    {
        b.ToTable("SupplierPayments");
        b.HasKey(x => x.Id);
        b.Property(x => x.PaymentNumber).IsRequired().HasMaxLength(50);
        b.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        b.Property(x => x.PaymentMethod).HasMaxLength(30);
        b.Property(x => x.Status).HasMaxLength(20);
        b.Property(x => x.ChequeNumber).HasMaxLength(50);
        b.Property(x => x.BankName).HasMaxLength(100);
        b.Property(x => x.Notes).HasMaxLength(500);
        b.Property(x => x.CreatedByUserId).HasMaxLength(450);
        b.HasIndex(x => x.SupplierId);
        b.HasIndex(x => x.PaymentDate);
        b.HasOne(x => x.Supplier).WithMany()
            .HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.PurchaseInvoice).WithMany()
            .HasForeignKey(x => x.PurchaseInvoiceId).OnDelete(DeleteBehavior.SetNull);
    }
}
