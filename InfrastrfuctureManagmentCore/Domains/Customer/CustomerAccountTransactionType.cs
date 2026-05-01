using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Customer
{
    public enum CustomerAccountTransactionType
    {
        OpeningDebit = 1,
        OpeningCredit = 2,
        SaleInvoice = 3,
        SalesReturn = 4,
        Receipt = 5,
        AdjustmentDebit = 6,
        AdjustmentCredit = 7
    }
}
