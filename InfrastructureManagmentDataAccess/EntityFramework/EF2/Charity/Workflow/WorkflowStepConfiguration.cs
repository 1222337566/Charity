using InfrastrfuctureManagmentCore.Domains.Charity.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Workflow
{
    public class WorkflowStepConfiguration : IEntityTypeConfiguration<WorkflowStep>
    {
        public void Configure(EntityTypeBuilder<WorkflowStep> b)
        {
            b.ToTable("WorkflowSteps");
            b.HasKey(x => x.Id);
            b.Property(x => x.EntityType).HasMaxLength(50).IsRequired();
            b.Property(x => x.EntityTitle).HasMaxLength(300);
            b.Property(x => x.StepName).HasMaxLength(100);
            b.Property(x => x.AssignedRole).HasMaxLength(100);
            b.Property(x => x.Status).HasMaxLength(30);
            b.Property(x => x.ActionNote).HasMaxLength(1000);

            b.HasIndex(x => new { x.EntityType, x.EntityId });
            b.HasIndex(x => new { x.AssignedRole, x.Status });

            // Indexes مهمة بعد زيادة البيانات التجريبية، وخصوصًا عند الضغط على موافقة والرجوع للوحة سير العمل.
            b.HasIndex(x => new { x.AssignedRole, x.Status, x.IsActive, x.CreatedAtUtc })
                .HasDatabaseName("IX_WorkflowSteps_AssignedRole_Status_IsActive_CreatedAtUtc");

            b.HasIndex(x => new { x.EntityType, x.EntityId, x.IsActive, x.StepOrder })
                .HasDatabaseName("IX_WorkflowSteps_EntityType_EntityId_IsActive_StepOrder");
        }
    }
}
