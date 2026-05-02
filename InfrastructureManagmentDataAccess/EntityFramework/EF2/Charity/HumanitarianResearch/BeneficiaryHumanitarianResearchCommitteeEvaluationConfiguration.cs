using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearchCommitteeEvaluationConfiguration : IEntityTypeConfiguration<BeneficiaryHumanitarianResearchCommitteeEvaluation>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryHumanitarianResearchCommitteeEvaluation> builder)
        {
            builder.ToTable("CharityBeneficiaryHumanitarianResearchCommitteeEvaluations");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Decision).HasMaxLength(50).IsRequired();
        }
    }
}
