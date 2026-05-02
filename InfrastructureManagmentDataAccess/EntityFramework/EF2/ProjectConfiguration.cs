using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    using InfrastrfuctureManagmentCore.Domains.Projects;
    using InfrastructureManagmentCore.Domains.Projects;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ProjectConfiguration : IEntityTypeConfiguration<Projectx>
    {
        public void Configure(EntityTypeBuilder<Projectx> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProjectCode)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.NameAr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.NameEn)
                .HasMaxLength(200);

            builder.Property(x => x.CustomerName)
                .HasMaxLength(200);

            builder.Property(x => x.Location)
                .HasMaxLength(300);

            builder.Property(x => x.Notes)
                .HasMaxLength(2000);

            builder.Property(x => x.EstimatedBudget)
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.ProjectCode).IsUnique();
        }
    }
}
