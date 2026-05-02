using InfrastructureManagmentCore.Domains.Identity;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class BeneficiaryConfiguration : IEntityTypeConfiguration<Beneficiary>
    {
        public void Configure(EntityTypeBuilder<Beneficiary> builder)
        {
            builder.ToTable("CharityBeneficiaries");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code).IsRequired().HasMaxLength(30);
            builder.Property(x => x.FullName).IsRequired().HasMaxLength(250);
            builder.Property(x => x.NationalId).HasMaxLength(14);
            builder.Property(x => x.PhoneNumber).HasMaxLength(30);
            builder.Property(x => x.AlternatePhoneNumber).HasMaxLength(30);
            builder.Property(x => x.AddressLine).HasMaxLength(500);
            builder.Property(x => x.IncomeSource).HasMaxLength(250);
            builder.Property(x => x.HealthStatus).HasMaxLength(250);
            builder.Property(x => x.EducationStatus).HasMaxLength(250);
            builder.Property(x => x.WorkStatus).HasMaxLength(250);
            builder.Property(x => x.HousingStatus).HasMaxLength(250);
            builder.Property(x => x.Notes).HasMaxLength(2000);
            builder.Property(x => x.MonthlyIncome).HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.Code).IsUnique();
            builder.HasIndex(x => x.PhoneNumber);
            builder.HasIndex(x => x.NationalId).IsUnique().HasFilter("[NationalId] IS NOT NULL");

            builder.HasOne(x => x.Gender).WithMany().HasForeignKey(x => x.GenderId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.MaritalStatus).WithMany().HasForeignKey(x => x.MaritalStatusId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Governorate).WithMany().HasForeignKey(x => x.GovernorateId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.City).WithMany().HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Area).WithMany().HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.Documents)
    .WithOne(x => x.Beneficiary)
    .HasForeignKey(x => x.BeneficiaryId)
    .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
