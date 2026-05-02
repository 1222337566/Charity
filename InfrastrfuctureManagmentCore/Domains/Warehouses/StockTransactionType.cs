using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Warehouses
{
    public enum StockTransactionType
    {
        OpeningBalance = 1,
        Purchase = 2,
        PurchaseReturn = 3,
        Sale = 4,
        SaleReturn = 5,
        TransferIn = 6,
        TransferOut = 7,
        AdjustmentIncrease = 8,
        AdjustmentDecrease = 9
    }
    
}
