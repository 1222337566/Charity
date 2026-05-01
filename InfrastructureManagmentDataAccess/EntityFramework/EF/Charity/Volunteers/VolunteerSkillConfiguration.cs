using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Volunteers
{
    public class VolunteerSkillConfiguration : IEntityTypeConfiguration<VolunteerSkill>
    {
        public void Configure(EntityTypeBuilder<VolunteerSkill> builder)
        {
            builder.ToTable("CharityVolunteerSkills");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SkillLevel).HasMaxLength(30).IsRequired();
            builder.Property(x => x.Notes).HasMaxLength(500);

            builder.HasOne(x => x.Volunteer)
                .WithMany()
                .HasForeignKey(x => x.VolunteerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.SkillDefinition)
                .WithMany()
                .HasForeignKey(x => x.SkillDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.VolunteerId, x.SkillDefinitionId }).IsUnique();
        }
    }
}
