using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.PaymentMethods
{
    
    public class EditPaymentMethodVm
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "كود الطريقة")]
        public string MethodCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "اسم الطريقة")]
        public string MethodNameAr { get; set; } = string.Empty;

        [Display(Name = "الاسم الإنجليزي")]
        public string? MethodNameEn { get; set; }

        [Display(Name = "نقدي")]
        public bool IsCash { get; set; }

        [Display(Name = "افتراضي")]
        public bool IsDefault { get; set; }

        [Display(Name = "نشطة")]
        public bool IsActive { get; set; }
    }
}
