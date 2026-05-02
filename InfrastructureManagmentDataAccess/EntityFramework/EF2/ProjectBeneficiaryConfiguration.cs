using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectBeneficiaryConfiguration : IEntityTypeConfiguration<ProjectBeneficiary>
    {
        public void Configure(EntityTypeBuilder<ProjectBeneficiary> builder)
        {
            builder.ToTable("CharityProjectBeneficiaries");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.BenefitType).HasMaxLength(100);
            builder.Property(x => x.Notes).HasMaxLength(2000);

            builder.HasOne(x => x.Project)
                .WithMany(x => x.Beneficiaries)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Beneficiary)
                .WithMany()
                .HasForeignKey(x => x.BeneficiaryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.ProjectId, x.BeneficiaryId }).IsUnique();
        }
    }
}
