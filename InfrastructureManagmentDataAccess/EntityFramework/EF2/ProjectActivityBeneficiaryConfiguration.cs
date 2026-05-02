using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectActivityBeneficiaryConfiguration
        : IEntityTypeConfiguration<ProjectActivityBeneficiary>
    {
        public void Configure(EntityTypeBuilder<ProjectActivityBeneficiary> b)
        {
            b.ToTable("CharityProjectActivityBeneficiaries");
            b.HasKey(x => x.Id);

            b.Property(x => x.TargetGroupName).HasMaxLength(200);
            b.Property(x => x.ParticipationType).HasMaxLength(50).HasDefaultValue("Beneficiary");
            b.Property(x => x.VerificationStatus).HasMaxLength(50).HasDefaultValue("Unverified");
            b.Property(x => x.VerificationNotes).HasMaxLength(1000);
            b.Property(x => x.Notes).HasMaxLength(2000);

            // منع تكرار نفس المستفيد في نفس النشاط
            b.HasIndex(x => new { x.ActivityId, x.BeneficiaryId }).IsUnique();
            b.HasIndex(x => x.ProjectId);
            b.HasIndex(x => x.PhaseId);
            b.HasIndex(x => x.VerificationStatus);

            b.HasOne(x => x.Project)
             .WithMany()
             .HasForeignKey(x => x.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Phase)
             .WithMany()
             .HasForeignKey(x => x.PhaseId)
             .OnDelete(DeleteBehavior.NoAction);

            b.HasOne(x => x.Activity)
             .WithMany()
             .HasForeignKey(x => x.ActivityId)
             .OnDelete(DeleteBehavior.NoAction);

            b.HasOne(x => x.Beneficiary)
             .WithMany()
             .HasForeignKey(x => x.BeneficiaryId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class ProjectActivityBeneficiaryAttachmentConfiguration
        : IEntityTypeConfiguration<ProjectActivityBeneficiaryAttachment>
    {
        public void Configure(EntityTypeBuilder<ProjectActivityBeneficiaryAttachment> b)
        {
            b.ToTable("CharityProjectActivityBeneficiaryAttachments");
            b.HasKey(x => x.Id);

            b.Property(x => x.AttachmentType).HasMaxLength(50);
            b.Property(x => x.OriginalFileName).HasMaxLength(500);
            b.Property(x => x.ContentType).HasMaxLength(200);
            b.Property(x => x.Notes).HasMaxLength(1000);

            b.HasIndex(x => x.ActivityBeneficiaryId);

            b.HasOne(x => x.ActivityBeneficiary)
             .WithMany(x => x.Attachments)
             .HasForeignKey(x => x.ActivityBeneficiaryId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
