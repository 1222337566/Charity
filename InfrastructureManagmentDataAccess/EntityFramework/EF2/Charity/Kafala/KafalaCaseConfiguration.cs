using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Kafala
{
    public class KafalaCaseConfiguration : IEntityTypeConfiguration<KafalaCase>
    {
        public void Configure(EntityTypeBuilder<KafalaCase> builder)
        {
            builder.ToTable("CharityKafalaCases");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CaseNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.SponsorshipType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Frequency).HasMaxLength(30).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(30).IsRequired();
            builder.Property(x => x.MonthlyAmount).HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.CaseNumber).IsUnique();
            builder.HasOne(x => x.Sponsor).WithMany(x => x.KafalaCases).HasForeignKey(x => x.SponsorId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.PaymentMethod).WithMany().HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.FinancialAccount).WithMany().HasForeignKey(x => x.FinancialAccountId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
