using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    using InfrastrfuctureManagmentCore.Domains.Expenses;
    using InfrastructureManagmentCore.Domains.Products;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ExpenseConfiguration : IEntityTypeConfiguration<Expensex>
    {
        public void Configure(EntityTypeBuilder<Expensex> builder)
        {
            builder.ToTable("Expenses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ExpenseNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.ExpenseNumber).IsUnique();

            builder.HasOne(x => x.ExpenseCategory)
                .WithMany()
                .HasForeignKey(x => x.ExpenseCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PaymentMethod)
                .WithMany()
                .HasForeignKey(x => x.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
