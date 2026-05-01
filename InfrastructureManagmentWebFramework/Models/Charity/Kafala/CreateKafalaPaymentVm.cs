using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Kafala
{
    public class CreateKafalaPaymentVm
    {
        [Required] public Guid KafalaCaseId { get; set; }
        [DataType(DataType.Date)] public DateTime PaymentDate { get; set; } = DateTime.Today;
        [Range(0.01, 999999999)] public decimal Amount { get; set; }
        [Required] public string PeriodLabel { get; set; } = string.Empty;
        [Required] public string Direction { get; set; } = "Received";
        public Guid? PaymentMethodId { get; set; }
        public Guid? FinancialAccountId { get; set; }
        public string? ReferenceNumber { get; set; }
        public string Status { get; set; } = "Confirmed";
        public string? Notes { get; set; }

        public List<SelectListItem> KafalaCases { get; set; } = new();
        public List<SelectListItem> PaymentMethods { get; set; } = new();
        public List<SelectListItem> FinancialAccounts { get; set; } = new();
        public List<SelectListItem> Directions { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }
}
