using InfrastructureManagmentCore.Domains.Identity;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class BeneficiaryFamilyMemberConfiguration : IEntityTypeConfiguration<BeneficiaryFamilyMember>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryFamilyMember> builder)
        {
            builder.ToTable("CharityBeneficiaryFamilyMembers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FullName).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Relationship).IsRequired().HasMaxLength(100);
            builder.Property(x => x.NationalId).HasMaxLength(14);
            builder.Property(x => x.EducationStatus).HasMaxLength(250);
            builder.Property(x => x.WorkStatus).HasMaxLength(250);
            builder.Property(x => x.HealthCondition).HasMaxLength(250);
            builder.Property(x => x.MonthlyIncome).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Notes).HasMaxLength(1000);

            builder.HasOne(x => x.Beneficiary)
                .WithMany(x => x.FamilyMembers)
                .HasForeignKey(x => x.BeneficiaryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Gender)
                .WithMany()
                .HasForeignKey(x => x.GenderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
