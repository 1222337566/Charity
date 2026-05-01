using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class CharityProjectConfiguration : IEntityTypeConfiguration<CharityProject>
    {
        public void Configure(EntityTypeBuilder<CharityProject> builder)
        {
            builder.ToTable("CharityProjects");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(4000);
            builder.Property(x => x.Status).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Location).HasMaxLength(500);
            builder.Property(x => x.Objectives).HasMaxLength(4000);
            builder.Property(x => x.Kpis).HasMaxLength(4000);
            builder.Property(x => x.Notes).HasMaxLength(4000);
            builder.Property(x => x.Budget).HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.Code).IsUnique();
            builder.HasIndex(x => x.Name);
            builder.HasIndex(x => new { x.Status, x.IsActive });
        }
    }
}
