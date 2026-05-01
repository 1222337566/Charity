using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.AccountingProjects
{
    public class CreateProjectAccountingProfileVm
    {
        [Required] public Guid ProjectId { get; set; }
        public Guid? DefaultCostCenterId { get; set; }
        public Guid? DefaultRevenueAccountId { get; set; }
        public Guid? DefaultExpenseAccountId { get; set; }
        public bool AutoTagJournalLinesWithProject { get; set; } = true;
        public bool AutoUseProjectCostCenter { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }

        public List<SelectListItem> Projects { get; set; } = new();
        public List<SelectListItem> CostCenters { get; set; } = new();
        public List<SelectListItem> Accounts { get; set; } = new();
    }
}
