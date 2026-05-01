using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    using InfrastrfuctureManagmentCore.Domains.Item;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class UnitConfiguration : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.ToTable("Units");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UnitCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.UnitNameAr)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.UnitNameEn)
                .HasMaxLength(100);

            builder.Property(x => x.Symbol)
                .HasMaxLength(20);

            builder.HasIndex(x => x.UnitCode).IsUnique();
        }
    }
}
