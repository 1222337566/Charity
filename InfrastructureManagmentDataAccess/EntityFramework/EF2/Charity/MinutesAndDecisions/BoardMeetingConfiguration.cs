using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.MinutesAndDecisions
{
    public class BoardMeetingConfiguration : IEntityTypeConfiguration<BoardMeeting>
    {
        public void Configure(EntityTypeBuilder<BoardMeeting> builder)
        {
            builder.ToTable("CharityBoardMeetings");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.MeetingNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(300).IsRequired();
            builder.Property(x => x.Location).HasMaxLength(250);
            builder.Property(x => x.MeetingType).HasMaxLength(100);
            builder.Property(x => x.Status).HasMaxLength(50);

            builder.HasIndex(x => x.MeetingNumber).IsUnique();

            builder.HasOne(x => x.Minute)
                .WithOne(x => x.BoardMeeting!)
                .HasForeignKey<BoardMeetingMinute>(x => x.BoardMeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Attendees)
                .WithOne(x => x.BoardMeeting!)
                .HasForeignKey(x => x.BoardMeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Decisions)
                .WithOne(x => x.BoardMeeting!)
                .HasForeignKey(x => x.BoardMeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Attachments)
                .WithOne(x => x.BoardMeeting!)
                .HasForeignKey(x => x.BoardMeetingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
