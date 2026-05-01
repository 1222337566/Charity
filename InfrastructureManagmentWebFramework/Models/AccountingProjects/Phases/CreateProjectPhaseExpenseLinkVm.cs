using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.AccountingProjects.Phases
{
    public class CreateProjectPhaseExpenseLinkVm
    {
        [Required] public Guid ExpenseId { get; set; }
        [Required] public Guid ProjectId { get; set; }
        [Required] public Guid ProjectPhaseId { get; set; }
        public Guid? ProjectBudgetLineId { get; set; }
        public Guid? CostCenterId { get; set; }
        public bool IncludeInActualCost { get; set; } = true;
        public string? Notes { get; set; }

        public List<SelectListItem> Expenses { get; set; } = new();
        public List<SelectListItem> Projects { get; set; } = new();
        public List<SelectListItem> Phases { get; set; } = new();
        public List<SelectListItem> BudgetLines { get; set; } = new();
        public List<SelectListItem> CostCenters { get; set; } = new();
    }
}
