using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Volunteers
{
    public class VolunteerHourLogConfiguration : IEntityTypeConfiguration<VolunteerHourLog>
    {
        public void Configure(EntityTypeBuilder<VolunteerHourLog> builder)
        {
            builder.ToTable("CharityVolunteerHourLogs");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActivityTitle).HasMaxLength(200).IsRequired();
            builder.Property(x => x.ProjectNameSnapshot).HasMaxLength(200);
            builder.Property(x => x.Hours).HasColumnType("decimal(10,2)");

            builder.HasOne(x => x.Volunteer)
                .WithMany(x => x.HourLogs)
                .HasForeignKey(x => x.VolunteerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Assignment)
                .WithMany(x => x.HourLogs)
                .HasForeignKey(x => x.AssignmentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => new { x.VolunteerId, x.WorkDate });
            builder.HasIndex(x => x.ProjectId);
        }
    }
}
