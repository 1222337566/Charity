using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.MinutesAndDecisions
{
    public class BoardMeetingAttendeeConfiguration : IEntityTypeConfiguration<BoardMeetingAttendee>
    {
        public void Configure(EntityTypeBuilder<BoardMeetingAttendee> builder)
        {
            builder.ToTable("CharityBoardMeetingAttendees");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FullName).HasMaxLength(250).IsRequired();
            builder.Property(x => x.PositionTitle).HasMaxLength(250);
            builder.Property(x => x.AttendanceStatus).HasMaxLength(50).IsRequired();
        }
    }
}
