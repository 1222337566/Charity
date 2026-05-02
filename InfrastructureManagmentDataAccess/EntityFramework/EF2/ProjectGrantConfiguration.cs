using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectGrantConfiguration : IEntityTypeConfiguration<ProjectGrant>
    {
        public void Configure(EntityTypeBuilder<ProjectGrant> builder)
        {
            builder.ToTable("CharityProjectGrants");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AllocatedAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Notes).HasMaxLength(2000);

            builder.HasOne(x => x.Project)
                .WithMany(x => x.Grants)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.GrantAgreement)
                .WithMany()
                .HasForeignKey(x => x.GrantAgreementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.ProjectId, x.GrantAgreementId }).IsUnique();
        }
    }
}
