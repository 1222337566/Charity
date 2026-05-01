using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Beneficiaries
{
    public class BeneficiaryCategoryAssignmentConfiguration : IEntityTypeConfiguration<BeneficiaryCategoryAssignment>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryCategoryAssignment> builder)
        {
            builder.ToTable("CharityBeneficiaryCategoryAssignments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.Property(x => x.CreatedByUserId)
                .HasMaxLength(450);

            builder.HasOne(x => x.Beneficiary)
                .WithMany()
                .HasForeignKey(x => x.BeneficiaryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Assignments)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.BeneficiaryId);
            builder.HasIndex(x => x.CategoryId);
            builder.HasIndex(x => x.ProjectId);
            builder.HasIndex(x => x.ProjectActivityId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.AssignedAtUtc);

            builder.HasIndex(x => new
            {
                x.BeneficiaryId,
                x.CategoryId,
                x.ProjectId,
                x.ProjectActivityId,
                x.Status
            });
        }
    }
}
