using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.POS
{
   
    public class PosPaymentVm
    {
        public Guid PaymentMethodId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public List<SelectListItem> PaymentMethods { get; set; } = new();
    }
}
