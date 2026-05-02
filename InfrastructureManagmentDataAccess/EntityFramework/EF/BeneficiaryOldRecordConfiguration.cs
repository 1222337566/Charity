using InfrastructureManagmentCore.Domains.Identity;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class BeneficiaryOldRecordConfiguration : IEntityTypeConfiguration<BeneficiaryOldRecord>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryOldRecord> builder)
        {
            builder.ToTable("CharityBeneficiaryOldRecords");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Details).HasMaxLength(4000);

            builder.HasOne(x => x.Beneficiary)
                .WithMany(x => x.OldRecords)
                .HasForeignKey(x => x.BeneficiaryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
