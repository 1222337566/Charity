using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.Charity.Kafala
{
    public class CreateKafalaSponsorVm
    {
        [Required] public string SponsorCode { get; set; } = string.Empty;
        [Required] public string FullName { get; set; } = string.Empty;
        [Required] public string SponsorType { get; set; } = "Individual";
        public string? NationalIdOrTaxNo { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? AddressLine { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public List<SelectListItem> SponsorTypes { get; set; } = new();
    }
}
