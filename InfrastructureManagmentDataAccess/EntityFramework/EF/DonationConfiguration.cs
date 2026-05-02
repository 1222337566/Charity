using InfrastructureManagmentCore.Domains.Identity;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class DonationConfiguration : IEntityTypeConfiguration<Donation>
    {
        public void Configure(EntityTypeBuilder<Donation> builder)
        {
            builder.ToTable("CharityDonations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DonationNumber).IsRequired().HasMaxLength(30);
            builder.Property(x => x.DonationType).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.CampaignName).HasMaxLength(200);
            builder.Property(x => x.ReceiptNumber).HasMaxLength(50);
            builder.Property(x => x.ReferenceNumber).HasMaxLength(50);
            builder.Property(x => x.Notes).HasMaxLength(2000);

            builder.HasIndex(x => x.DonationNumber).IsUnique();
            builder.HasIndex(x => x.DonationDate);
            builder.HasIndex(x => x.ReceiptNumber);

            builder.HasOne(x => x.Donor)
                .WithMany(x => x.Donations)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.PaymentMethod)
                .WithMany()
                .HasForeignKey(x => x.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FinancialAccount)
                .WithMany()
                .HasForeignKey(x => x.FinancialAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
