using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Kafala
{
    public class CreateKafalaCaseVm
    {
        [Required] public string CaseNumber { get; set; } = string.Empty;
        [Required] public Guid SponsorId { get; set; }
        [Required] public Guid BeneficiaryId { get; set; }
        [Required] public string SponsorshipType { get; set; } = "Orphan";
        [Required] public string Frequency { get; set; } = "Monthly";
        [Range(0.01, 999999999)] public decimal MonthlyAmount { get; set; }
        [DataType(DataType.Date)] public DateTime StartDate { get; set; } = DateTime.Today;
        [DataType(DataType.Date)] public DateTime? EndDate { get; set; }
        [DataType(DataType.Date)] public DateTime? NextDueDate { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public Guid? FinancialAccountId { get; set; }
        public bool AutoIncludeInAidCycles { get; set; } = true;
        public string Status { get; set; } = "Active";
        public string? Notes { get; set; }

        public List<SelectListItem> Sponsors { get; set; } = new();
        public List<SelectListItem> Beneficiaries { get; set; } = new();
        public List<SelectListItem> PaymentMethods { get; set; } = new();
        public List<SelectListItem> FinancialAccounts { get; set; } = new();
        public List<SelectListItem> SponsorshipTypes { get; set; } = new();
        public List<SelectListItem> Frequencies { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }
}
