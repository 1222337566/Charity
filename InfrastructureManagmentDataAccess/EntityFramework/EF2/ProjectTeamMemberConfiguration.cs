using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectTeamMemberConfiguration
        : IEntityTypeConfiguration<ProjectTeamMember>
    {
        public void Configure(EntityTypeBuilder<ProjectTeamMember> b)
        {
            b.ToTable("CharityProjectTeamMembers");
            b.HasKey(x => x.Id);

            b.Property(x => x.MemberType).HasMaxLength(20).HasDefaultValue("Volunteer");
            b.Property(x => x.RoleTitle).HasMaxLength(200);
            b.Property(x => x.ParticipationStatus).HasMaxLength(30).HasDefaultValue("Assigned");
            b.Property(x => x.VerificationStatus).HasMaxLength(30).HasDefaultValue("Unverified");
            b.Property(x => x.VerificationNotes).HasMaxLength(1000);
            b.Property(x => x.Notes).HasMaxLength(2000);

            b.HasIndex(x => new { x.ProjectId, x.MemberType });
            b.HasIndex(x => x.PhaseId);
            b.HasIndex(x => x.ActivityId);
            b.HasIndex(x => x.EmployeeId);
            b.HasIndex(x => x.VolunteerId);

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

            b.HasOne(x => x.Employee)
             .WithMany()
             .HasForeignKey(x => x.EmployeeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Volunteer)
             .WithMany()
             .HasForeignKey(x => x.VolunteerId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class ProjectTeamMemberAttachmentConfiguration
        : IEntityTypeConfiguration<ProjectTeamMemberAttachment>
    {
        public void Configure(EntityTypeBuilder<ProjectTeamMemberAttachment> b)
        {
            b.ToTable("CharityProjectTeamMemberAttachments");
            b.HasKey(x => x.Id);

            b.Property(x => x.AttachmentType).HasMaxLength(50);
            b.Property(x => x.OriginalFileName).HasMaxLength(500);
            b.Property(x => x.ContentType).HasMaxLength(200);
            b.Property(x => x.Notes).HasMaxLength(1000);

            b.HasIndex(x => x.TeamMemberId);

            b.HasOne(x => x.TeamMember)
             .WithMany(x => x.Attachments)
             .HasForeignKey(x => x.TeamMemberId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
