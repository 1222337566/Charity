using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.AccountingProjects
{
    public class ProjectLedgerVm
    {
        public Guid? ProjectId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<SelectListItem> Projects { get; set; } = new();
        public ProjectLedgerDto? Ledger { get; set; }
    }
}
