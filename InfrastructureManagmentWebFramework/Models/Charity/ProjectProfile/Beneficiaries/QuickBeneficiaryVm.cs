using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Beneficiaries
{
    public class QuickBeneficiaryVm
    {
        public Guid ProjectId { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "الاسم مطلوب")]
        public string FullName { get; set; } = "";
        public string? NationalId { get; set; }
        public string? PhoneNumber { get; set; }
        public int FamilyMembersCount { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public string? BenefitType { get; set; }
        public string? TargetGroupName { get; set; }   // ← الفئة المستهدفة
        public string? Notes { get; set; }
        public bool AddAnother { get; set; }
        public List<string> TargetGroups { get; set; } = new();  // ← قائمة الفئات من المقترح
    }
}
