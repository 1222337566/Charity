using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.MinutesAndDecisions
{
    public class BoardDecisionConfiguration : IEntityTypeConfiguration<BoardDecision>
    {
        public void Configure(EntityTypeBuilder<BoardDecision> builder)
        {
            builder.ToTable("CharityBoardDecisions");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.DecisionNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(300).IsRequired();
            builder.Property(x => x.DecisionKind).HasMaxLength(50).IsRequired();
            builder.Property(x => x.ResponsibleParty).HasMaxLength(250);
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Priority).HasMaxLength(50).IsRequired();
            builder.Property(x => x.RelatedEntityType).HasMaxLength(100);

            builder.HasIndex(x => x.DecisionNumber).IsUnique();

            builder.HasMany(x => x.FollowUps)
                .WithOne(x => x.BoardDecision!)
                .HasForeignKey(x => x.BoardDecisionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
