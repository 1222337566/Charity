using InfrastrfuctureManagmentCore.Domains.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Sale
{
    public class SalesInvoicePayment
    {
        public Guid Id { get; set; }

        public Guid SalesInvoiceId { get; set; }
        public SalesInvoice? SalesInvoice { get; set; }

        public Guid PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public decimal Amount { get; set; }
    }
}
