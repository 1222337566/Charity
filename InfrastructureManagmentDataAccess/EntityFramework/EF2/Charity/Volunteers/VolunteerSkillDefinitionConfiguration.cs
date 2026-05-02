using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Volunteers
{
    public class VolunteerSkillDefinitionConfiguration : IEntityTypeConfiguration<VolunteerSkillDefinition>
    {
        public void Configure(EntityTypeBuilder<VolunteerSkillDefinition> builder)
        {
            builder.ToTable("CharityVolunteerSkillDefinitions");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Category).HasMaxLength(100);
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}
