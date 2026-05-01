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

    public class ItemGroupConfiguration : IEntityTypeConfiguration<ItemGroup>
    {
        public void Configure(EntityTypeBuilder<ItemGroup> builder)
        {
            builder.ToTable("ItemGroups");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.GroupCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.GroupNameAr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.GroupNameEn)
                .HasMaxLength(200);

            builder.HasIndex(x => x.GroupCode).IsUnique();

            builder.HasOne(x => x.ParentGroup)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentGroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
