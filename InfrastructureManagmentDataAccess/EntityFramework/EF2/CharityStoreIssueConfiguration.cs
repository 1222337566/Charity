using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class CharityStoreIssueConfiguration : IEntityTypeConfiguration<CharityStoreIssue>
    {
        public void Configure(EntityTypeBuilder<CharityStoreIssue> builder)
        {
            builder.ToTable("CharityStoreIssues");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.IssueNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.IssueType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.IssuedToName).HasMaxLength(200);
            builder.Property(x => x.Notes).HasMaxLength(2000);
            builder.Property(x => x.ApprovalStatus).HasMaxLength(30);
            builder.Property(x => x.ApprovedByUserId).HasMaxLength(450);
            builder.Property(x => x.RejectedByUserId).HasMaxLength(450);
            builder.Property(x => x.ApprovalNotes).HasMaxLength(2000);
            builder.Property(x => x.CreatedByUserId).HasMaxLength(450);
            builder.Property(x => x.SourceId);
            builder.HasIndex(x => new { x.SourceType, x.SourceId });
            builder.HasIndex(x => x.IssueNumber).IsUnique();
            builder.HasIndex(x => new { x.WarehouseId, x.IssueDate });
            builder.HasIndex(x => x.ApprovalStatus);

            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Beneficiary)
                .WithMany()
                .HasForeignKey(x => x.BeneficiaryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Lines)
                .WithOne(x => x.Issue)
                .HasForeignKey(x => x.IssueId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
