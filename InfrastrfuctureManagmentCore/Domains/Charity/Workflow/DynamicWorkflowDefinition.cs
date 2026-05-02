namespace InfrastrfuctureManagmentCore.Domains.Charity.Workflow
{
    /// <summary>
    /// تعريف مسار موافقة ديناميكي مُخزَّن في قاعدة البيانات.
    /// يتيح تعديل مسارات الموافقة بدون تعديل الكود.
    /// عند وجود تعريف نشط لنوع الكيان، يُستخدم بدل التعريف الثابت في WorkflowDefinition.
    /// </summary>
    public class DynamicWorkflowDefinition
    {
        public Guid   Id          { get; set; }
        public string EntityType  { get; set; } = null!;   // AidRequest | KafalaCase | ...
        public string DisplayName { get; set; } = null!;
        public bool   IsActive    { get; set; } = true;
        public string? Notes      { get; set; }
        public DateTime CreatedAtUtc    { get; set; } = DateTime.UtcNow;
        public string?  CreatedByUserId { get; set; }

        public ICollection<DynamicWorkflowStepTemplate> Steps { get; set; }
            = new List<DynamicWorkflowStepTemplate>();
    }
}
