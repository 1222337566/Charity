using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastructureManagmentCore.Domains.Profiling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
   

    public class CustomerConfiguration : IEntityTypeConfiguration<CustomerClient>
    {
        public void Configure(EntityTypeBuilder<CustomerClient> builder)
        {
            builder.ToTable("CustomerClients");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerNumber)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.NameAr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.NameEn)
                .HasMaxLength(200);

            builder.Property(x => x.Tel)
                .HasMaxLength(50);

            builder.Property(x => x.MobileNo)
                .HasMaxLength(50);

            builder.Property(x => x.Remarks)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.CustomerNumber).IsUnique();
        }
    }
}
