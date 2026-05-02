using InfrastrfuctureManagmentCore.Domains.Company;
using InfrastrfuctureManagmentCore.Domains.Sale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentCore.Models.Print
{
    public class ReceiptPrintVm
    {
        public SalesInvoice Invoice { get; set; } = new SalesInvoice();
        public CompanyProfile? Company { get; set; }
    }
}
