using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class GrantConditionConfiguration : IEntityTypeConfiguration<GrantCondition>
    {
        public void Configure(EntityTypeBuilder<GrantCondition> builder)
        {
            builder.ToTable("CharityGrantConditions");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ConditionTitle).HasMaxLength(300).IsRequired();
            builder.Property(x => x.ConditionDetails).HasMaxLength(4000).IsRequired();
            builder.Property(x => x.Notes).HasMaxLength(4000);

            builder.HasOne(x => x.GrantAgreement)
                .WithMany(x => x.Conditions)
                .HasForeignKey(x => x.GrantAgreementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.GrantAgreementId, x.IsFulfilled, x.IsMandatory });
        }
    }
}
