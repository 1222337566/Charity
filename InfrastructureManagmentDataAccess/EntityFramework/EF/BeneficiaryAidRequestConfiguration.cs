using InfrastructureManagmentCore.Domains.Identity;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class BeneficiaryAidRequestConfiguration : IEntityTypeConfiguration<BeneficiaryAidRequest>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryAidRequest> builder)
        {
            builder.ToTable("CharityAidRequests");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequestedAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Reason).HasMaxLength(2000);
            builder.Property(x => x.UrgencyLevel).HasMaxLength(50);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);

            builder.HasOne(x => x.Beneficiary)
                .WithMany(x => x.AidRequests)
                .HasForeignKey(x => x.BeneficiaryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.AidType)
                .WithMany()
                .HasForeignKey(x => x.AidTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
