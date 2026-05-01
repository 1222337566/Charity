using System;

namespace InfrastructureManagmentWebFramework.Models.Accounting
{
    public class JournalEntryLineListItemVm
    {
        public Guid Id { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string AccountNameAr { get; set; } = string.Empty;
        public string? CostCenterName { get; set; }
        public Guid? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
    }
}
