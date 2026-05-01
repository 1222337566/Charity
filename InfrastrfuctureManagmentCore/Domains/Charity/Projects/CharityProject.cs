namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    public class CharityProject
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Budget { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? TargetBeneficiariesCount { get; set; }
        public string? Location { get; set; }
        public string? Objectives { get; set; }
        public string? Kpis { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<ProjectGoal>      Goals      { get; set; } = new List<ProjectGoal>();
        public ICollection<ProjectBudgetLine> BudgetLines { get; set; } = new List<ProjectBudgetLine>();
        public ICollection<ProjectActivity> Activities { get; set; } = new List<ProjectActivity>();
        public ICollection<ProjectBeneficiary> Beneficiaries { get; set; } = new List<ProjectBeneficiary>();
        public ICollection<ProjectGrant> Grants { get; set; } = new List<ProjectGrant>();
    }
}
