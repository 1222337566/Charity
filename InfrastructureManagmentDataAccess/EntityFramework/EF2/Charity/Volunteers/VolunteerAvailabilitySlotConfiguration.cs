using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Volunteers
{
    public class VolunteerAvailabilitySlotConfiguration : IEntityTypeConfiguration<VolunteerAvailabilitySlot>
    {
        public void Configure(EntityTypeBuilder<VolunteerAvailabilitySlot> builder)
        {
            builder.ToTable("CharityVolunteerAvailabilitySlots");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DayOfWeekName).HasMaxLength(20).IsRequired();
            builder.Property(x => x.AvailabilityType).HasMaxLength(30).IsRequired();
            builder.Property(x => x.Area).HasMaxLength(150);
            builder.Property(x => x.Notes).HasMaxLength(500);

            builder.HasOne(x => x.Volunteer)
                .WithMany()
                .HasForeignKey(x => x.VolunteerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
