using InfrastrfuctureManagmentCore.Domains.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class CostCenterConfiguration : IEntityTypeConfiguration<CostCenter>
    {
        public void Configure(EntityTypeBuilder<CostCenter> builder)
        {
            builder.ToTable("AccountingCostCenters");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CostCenterCode).IsRequired().HasMaxLength(20);
            builder.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
            builder.Property(x => x.CostCenterNameEn).HasMaxLength(200);
            builder.Property(x => x.Notes).HasMaxLength(1000);

            builder.HasIndex(x => x.CostCenterCode).IsUnique();

            builder.HasOne(x => x.ParentCostCenter)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentCostCenterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
