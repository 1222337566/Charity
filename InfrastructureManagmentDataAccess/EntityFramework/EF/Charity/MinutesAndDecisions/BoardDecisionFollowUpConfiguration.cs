using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.MinutesAndDecisions
{
    public class BoardDecisionFollowUpConfiguration : IEntityTypeConfiguration<BoardDecisionFollowUp>
    {
        public void Configure(EntityTypeBuilder<BoardDecisionFollowUp> builder)
        {
            builder.ToTable("CharityBoardDecisionFollowUps");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        }
    }
}
