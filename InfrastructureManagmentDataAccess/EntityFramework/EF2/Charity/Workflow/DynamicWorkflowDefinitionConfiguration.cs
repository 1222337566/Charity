using InfrastrfuctureManagmentCore.Domains.Charity.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Workflow
{
    public class DynamicWorkflowDefinitionConfiguration : IEntityTypeConfiguration<DynamicWorkflowDefinition>
    {
        public void Configure(EntityTypeBuilder<DynamicWorkflowDefinition> b)
        {
            b.ToTable("CharityDynamicWorkflowDefinitions");
            b.HasKey(x => x.Id);
            b.Property(x => x.EntityType).HasMaxLength(80).IsRequired();
            b.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            b.Property(x => x.Notes).HasMaxLength(1000);
            b.Property(x => x.CreatedByUserId).HasMaxLength(450);
            b.HasIndex(x => new { x.EntityType, x.IsActive });

            b.HasMany(x => x.Steps)
             .WithOne(x => x.Definition)
             .HasForeignKey(x => x.DefinitionId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class DynamicWorkflowStepTemplateConfiguration : IEntityTypeConfiguration<DynamicWorkflowStepTemplate>
    {
        public void Configure(EntityTypeBuilder<DynamicWorkflowStepTemplate> b)
        {
            b.ToTable("CharityDynamicWorkflowStepTemplates");
            b.HasKey(x => x.Id);
            b.Property(x => x.StepName).HasMaxLength(200).IsRequired();
            b.Property(x => x.AssignedRole).HasMaxLength(100).IsRequired();
            b.Property(x => x.Description).HasMaxLength(500);
            b.HasIndex(x => new { x.DefinitionId, x.StepOrder });
        }
    }
}
