using InfrastrfuctureManagmentCore.Domains.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class HrEmployeeConfiguration : IEntityTypeConfiguration<HrEmployee>
    {
        public void Configure(EntityTypeBuilder<HrEmployee> builder)
        {
            builder.ToTable("CharityHrEmployees");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
            builder.Property(x => x.FullName).HasMaxLength(250).IsRequired();
            builder.Property(x => x.NationalId).HasMaxLength(50);
            builder.Property(x => x.PhoneNumber).HasMaxLength(50);
            builder.Property(x => x.Email).HasMaxLength(200);
            builder.Property(x => x.AddressLine).HasMaxLength(500);
            builder.Property(x => x.EmploymentType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.Property(x => x.BankName).HasMaxLength(200);
            builder.Property(x => x.BankAccountNumber).HasMaxLength(100);
            builder.Property(x => x.UserId).HasMaxLength(450);
            builder.Property(x => x.Notes).HasMaxLength(2000);
            builder.Property(x => x.BasicSalary).HasColumnType("decimal(18,2)");
            builder.Property(x => x.InsuranceSalary).HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.Code).IsUnique();
            builder.HasIndex(x => x.NationalId);
            builder.HasIndex(x => x.PhoneNumber);

            builder.HasOne(x => x.Department)
                .WithMany(x => x.Employees)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.JobTitle)
                .WithMany(x => x.Employees)
                .HasForeignKey(x => x.JobTitleId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
