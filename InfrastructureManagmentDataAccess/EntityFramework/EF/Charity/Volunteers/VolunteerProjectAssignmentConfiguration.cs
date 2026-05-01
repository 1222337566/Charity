using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Volunteers
{
    public class VolunteerProjectAssignmentConfiguration : IEntityTypeConfiguration<VolunteerProjectAssignment>
    {
        public void Configure(EntityTypeBuilder<VolunteerProjectAssignment> builder)
        {
            builder.ToTable("CharityVolunteerProjectAssignments");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RoleTitle).HasMaxLength(150).IsRequired();
            builder.Property(x => x.AssignmentType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.Property(x => x.ProjectNameSnapshot).HasMaxLength(200);

            builder.HasOne(x => x.Volunteer)
                .WithMany(x => x.Assignments)
                .HasForeignKey(x => x.VolunteerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.VolunteerId, x.Status });
            builder.HasIndex(x => x.ProjectId);
        }
    }
}
