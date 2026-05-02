using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.Sale
{

    public class SalesPaymentVm
    {
        [Required]
        public Guid PaymentMethodId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        public List<SelectListItem> PaymentMethods { get; set; } = new();
    }
}
