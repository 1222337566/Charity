using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{


    public class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfile>
    {
        public void Configure(EntityTypeBuilder<CompanyProfile> builder)
        {
            builder.ToTable("CompanyProfiles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompanyNameAr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.CompanyNameEn)
                .HasMaxLength(200);

            builder.Property(x => x.Phone)
                .HasMaxLength(50);

            builder.Property(x => x.Address)
                .HasMaxLength(500);

            builder.Property(x => x.TaxNumber)
                .HasMaxLength(100);

            builder.Property(x => x.ReceiptHeaderText)
                .HasMaxLength(500);

            builder.Property(x => x.ReceiptFooterText)
                .HasMaxLength(500);
        }
    }
}
