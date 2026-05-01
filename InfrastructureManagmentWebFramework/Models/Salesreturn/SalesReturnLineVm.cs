using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Salesreturn
{
  
    public class SalesReturnLineVm
    {
        public Guid OriginalSalesInvoiceLineId { get; set; }
        public Guid ItemId { get; set; }

        public string ItemCode { get; set; } = string.Empty;
        public string ItemNameAr { get; set; } = string.Empty;

        public decimal SoldQuantity { get; set; }
        public decimal AlreadyReturnedQuantity { get; set; }
        public decimal RemainingQuantity { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ReturnQuantity { get; set; }
    }
}
