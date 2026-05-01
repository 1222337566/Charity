namespace InfrastrfuctureManagmentCore.Domains.Charity.Workflow
{
    /// <summary>
    /// تعريف مسار الموافقة لكل نوع طلب
    /// </summary>
    public static class WorkflowDefinition
    {
        public static List<WorkflowStepTemplate> GetSteps(string entityType) => entityType switch
        {
            "AidRequest" => new()
            {
                new(1, "مراجعة أولية",    "BeneficiariesOfficer"),
                new(2, "باحث اجتماعي",   "SocialResearcher"),
                new(3, "قرار اللجنة",    "CharityManager"),
                new(4, "اعتماد مالي",    "FinancialOfficer"),
            },
            "KafalaCase" => new()
            {
                new(1, "مراجعة الكفالة",  "BeneficiariesOfficer"),
                new(2, "اعتماد المدير",   "CharityManager"),
            },
            "AidCycle" => new()
            {
                new(1, "مراجعة الدورة",   "BeneficiariesOfficer"),
                new(2, "اعتماد الصرف",    "FinancialOfficer"),
                new(3, "موافقة المدير",   "CharityManager"),
            },
            "HumanitarianResearch" => new()
            {
                new(1, "مراجعة أولية",             "BeneficiariesOfficer"),
                new(2, "مراجعة البحث الاجتماعي",  "SocialResearcher"),
                new(3, "اعتماد الإحالة للجنة",     "CharityManager"),
            },
            "ProjectProposal" => new()
            {
                new(1, "مراجعة المقترح",   "ProjectManager"),
                new(2, "تقييم الميزانية",   "FinancialOfficer"),
                new(3, "اعتماد المدير",     "CharityManager"),
            },
            "StockNeedRequest" => new()
            {
                new(1, "مراجعة الطلب",    "StoreKeeper"),
                new(2, "اعتماد الشراء",   "FinancialOfficer"),
                new(3, "موافقة المدير",   "CharityManager"),
            },
            _ => new()
        };
    }

    public record WorkflowStepTemplate(int Order, string Name, string Role);
}
