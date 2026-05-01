using InfrastrfuctureManagmentCore.Domains.HR.Advanced;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.HR.Advanced
{
    public class HrPerformanceEvaluationConfiguration : IEntityTypeConfiguration<HrPerformanceEvaluation>
    {
        public void Configure(EntityTypeBuilder<HrPerformanceEvaluation> builder)
        {
            builder.ToTable("HrPerformanceEvaluations");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.EvaluationPeriod).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Result).HasMaxLength(30).IsRequired();
            builder.Property(x => x.DisciplineScore).HasColumnType("decimal(5,2)");
            builder.Property(x => x.PerformanceScore).HasColumnType("decimal(5,2)");
            builder.Property(x => x.CooperationScore).HasColumnType("decimal(5,2)");
            builder.Property(x => x.InitiativeScore).HasColumnType("decimal(5,2)");
            builder.Property(x => x.TotalScore).HasColumnType("decimal(5,2)");
            builder.Property(x => x.CreatedByUserId).HasMaxLength(450);
            builder.HasIndex(x => new { x.EmployeeId, x.EvaluationDate });
        }
    }
}
