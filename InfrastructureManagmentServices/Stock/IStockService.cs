using InfrastrfuctureManagmentCore.Domains.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Stock
{
    public interface IStockService
    {
        Task AddOpeningBalanceAsync(StockOperationRequest request);
        Task AddPurchaseAsync(StockOperationRequest request);
        Task AddPurchaseReturnAsync(StockOperationRequest request);

        Task AddSaleAsync(StockOperationRequest request);
        Task AddSaleReturnAsync(StockOperationRequest request);

        Task AddAdjustmentIncreaseAsync(StockOperationRequest request);
        Task AddAdjustmentDecreaseAsync(StockOperationRequest request);

        Task TransferAsync(StockTransferRequest request);
    }
}
