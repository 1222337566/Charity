using InfrastrfuctureManagmentCore.Domains.Accounting.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class AccountingIntegrationSourceDefinitionConfiguration : IEntityTypeConfiguration<AccountingIntegrationSourceDefinition>
    {
        public void Configure(EntityTypeBuilder<AccountingIntegrationSourceDefinition> builder)
        {
            builder.ToTable("AccountingIntegrationSourceDefinitions");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SourceType).HasMaxLength(100).IsRequired();
            builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
            builder.Property(x => x.NameEn).HasMaxLength(200);
            builder.Property(x => x.ModuleCode).HasMaxLength(100);
            builder.Property(x => x.Description).HasMaxLength(1000);

            builder.Property(x => x.EntityClrTypeName).HasMaxLength(500);
            builder.Property(x => x.IdPropertyName).HasMaxLength(100);
            builder.Property(x => x.DatePropertyName).HasMaxLength(100);
            builder.Property(x => x.AmountPropertyName).HasMaxLength(100);
            builder.Property(x => x.NumberPropertyName).HasMaxLength(100);
            builder.Property(x => x.TitlePropertyName).HasMaxLength(100);
            builder.Property(x => x.DescriptionTemplate).HasMaxLength(1000);
            builder.Property(x => x.FinancialAccountIdPropertyName).HasMaxLength(100);
            builder.Property(x => x.ProjectIdPropertyName).HasMaxLength(100);
            builder.Property(x => x.CostCenterIdPropertyName).HasMaxLength(100);
            builder.Property(x => x.EventCodePropertyName).HasMaxLength(100);
            builder.Property(x => x.DonationTypePropertyName).HasMaxLength(100);
            builder.Property(x => x.TargetingScopeCodePropertyName).HasMaxLength(100);
            builder.Property(x => x.PurposeNamePropertyName).HasMaxLength(100);
            builder.Property(x => x.AidTypeIdPropertyName).HasMaxLength(100);
            builder.Property(x => x.StoreMovementTypePropertyName).HasMaxLength(100);

            builder.HasIndex(x => x.SourceType).IsUnique();
            builder.HasIndex(x => x.IsActive);
            builder.HasIndex(x => new { x.SourceType, x.IsDynamicPostingEnabled });
        }
    }
}
