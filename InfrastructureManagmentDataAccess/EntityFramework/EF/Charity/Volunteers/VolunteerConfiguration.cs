using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Volunteers
{
    public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
    {
        public void Configure(EntityTypeBuilder<Volunteer> builder)
        {
            builder.ToTable("CharityVolunteers");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.VolunteerCode).HasMaxLength(50).IsRequired();
            builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Qualification).HasMaxLength(200);
            builder.Property(x => x.AddressLine).HasMaxLength(300);
            builder.Property(x => x.Nationality).HasMaxLength(100);
            builder.Property(x => x.NationalId).HasMaxLength(20);
            builder.Property(x => x.PhoneNumber).HasMaxLength(30);
            builder.Property(x => x.Email).HasMaxLength(150);
            builder.Property(x => x.Gender).HasMaxLength(20);
            builder.Property(x => x.MaritalStatus).HasMaxLength(30);
            builder.Property(x => x.PreferredArea).HasMaxLength(150);
            builder.Property(x => x.CreatedByUserId).HasMaxLength(450);

            builder.HasIndex(x => x.VolunteerCode).IsUnique();
            builder.HasIndex(x => x.NationalId);
            builder.HasIndex(x => x.PhoneNumber);
        }
    }
}
