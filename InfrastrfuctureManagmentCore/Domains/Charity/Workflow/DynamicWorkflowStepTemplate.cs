namespace InfrastrfuctureManagmentCore.Domains.Charity.Workflow
{
    /// <summary>
    /// خطوة واحدة داخل تعريف مسار الموافقة الديناميكي
    /// </summary>
    public class DynamicWorkflowStepTemplate
    {
        public Guid   Id           { get; set; }
        public Guid   DefinitionId { get; set; }
        public DynamicWorkflowDefinition Definition { get; set; } = null!;

        public int    StepOrder    { get; set; }
        public string StepName     { get; set; } = null!;
        public string AssignedRole { get; set; } = null!;
        public string? Description { get; set; }
    }
}
