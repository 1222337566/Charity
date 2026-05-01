using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.MinutesAndDecisions
{
    public class BoardMeetingMinuteConfiguration : IEntityTypeConfiguration<BoardMeetingMinute>
    {
        public void Configure(EntityTypeBuilder<BoardMeetingMinute> builder)
        {
            builder.ToTable("CharityBoardMeetingMinutes");
            builder.HasKey(x => x.Id);
        }
    }
}
