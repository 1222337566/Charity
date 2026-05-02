using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace InfrastructureManagmentServices.Stock
{
    using InfrastrfuctureManagmentCore.Domains.Warehouses;
    using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
    using InfrastrfuctureManagmentCore.Persistence.Repositories.Warehouses;
    using InfrastructureManagmentDataAccess.EntityFramework;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;

    public class StockService : IStockService
    {
        private readonly AppDbContext _db;
        private readonly IItemRepository _itemRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IStockTransactionRepository _stockTransactionRepository;
        private readonly IItemWarehouseBalanceRepository _itemWarehouseBalanceRepository;

        public StockService(
            AppDbContext db,
            IItemRepository itemRepository,
            IWarehouseRepository warehouseRepository,
            IStockTransactionRepository stockTransactionRepository,
            IItemWarehouseBalanceRepository itemWarehouseBalanceRepository)
        {
            _db = db;
            _itemRepository = itemRepository;
            _warehouseRepository = warehouseRepository;
            _stockTransactionRepository = stockTransactionRepository;
            _itemWarehouseBalanceRepository = itemWarehouseBalanceRepository;
        }

        public async Task AddOpeningBalanceAsync(StockOperationRequest request)
        {
            await ExecuteIncreaseAsync(request, StockTransactionType.OpeningBalance);
        }

        public async Task AddPurchaseAsync(StockOperationRequest request)
        {
            await ExecuteIncreaseAsync(request, StockTransactionType.Purchase);
        }

        public async Task AddPurchaseReturnAsync(StockOperationRequest request)
        {
            await ExecuteDecreaseAsync(request, StockTransactionType.PurchaseReturn);
        }

        public async Task AddSaleAsync(StockOperationRequest request)
        {
            await ExecuteDecreaseAsync(request, StockTransactionType.Sale);
        }

        public async Task AddSaleReturnAsync(StockOperationRequest request)
        {
            await ExecuteIncreaseAsync(request, StockTransactionType.SaleReturn);
        }

        public async Task AddAdjustmentIncreaseAsync(StockOperationRequest request)
        {
            await ExecuteIncreaseAsync(request, StockTransactionType.AdjustmentIncrease);
        }

        public async Task AddAdjustmentDecreaseAsync(StockOperationRequest request)
        {
            await ExecuteDecreaseAsync(request, StockTransactionType.AdjustmentDecrease);
        }

        public async Task TransferAsync(StockTransferRequest request)
        {
            ValidateTransferRequest(request);

            var item = await _itemRepository.GetByIdAsync(request.ItemId);
            if (item == null)
                throw new InvalidOperationException("الصنف غير موجود");

            if (!item.IsActive)
                throw new InvalidOperationException("الصنف غير نشط");

            if (item.IsService)
                throw new InvalidOperationException("لا يمكن عمل حركة مخزنية على صنف خدمي");

            if (!item.IsStockItem)
                throw new InvalidOperationException("الصنف غير مخزني");

            var fromWarehouse = await _warehouseRepository.GetByIdAsync(request.FromWarehouseId);
            if (fromWarehouse == null || !fromWarehouse.IsActive)
                throw new InvalidOperationException("مخزن الصرف غير موجود أو غير نشط");

            var toWarehouse = await _warehouseRepository.GetByIdAsync(request.ToWarehouseId);
            if (toWarehouse == null || !toWarehouse.IsActive)
                throw new InvalidOperationException("مخزن الإضافة غير موجود أو غير نشط");

            var hasOuterTransaction = _db.Database.CurrentTransaction != null;
            IDbContextTransaction? trx = null;

            if (!hasOuterTransaction)
                trx = await _db.Database.BeginTransactionAsync();

            try
            {
                var fromBalance = await GetOrCreateBalanceAsync(request.ItemId, request.FromWarehouseId);
                EnsureEnoughStock(fromBalance, request.Quantity);
                var operationUnitCost = request.UnitCost;

                if (operationUnitCost <= 0)
                {
                    operationUnitCost = await CalculateMovingAverageUnitCostAsync(
                        request.ItemId,
                        request.FromWarehouseId,
                        request.TransactionDateUtc ?? DateTime.UtcNow);
                }
                // صرف من المخزن المصدر
                fromBalance.QuantityOnHand -= request.Quantity;
                fromBalance.AvailableQuantity = fromBalance.QuantityOnHand - fromBalance.ReservedQuantity;
                fromBalance.LastUpdatedUtc = DateTime.UtcNow;
                await _itemWarehouseBalanceRepository.UpdateAsync(fromBalance);

                var transferOut = new StockTransaction
                {
                    Id = Guid.NewGuid(),
                    ItemId = request.ItemId,
                    ReferenceId = request.ReferenceId,
                    WarehouseId = request.FromWarehouseId,
                    RelatedWarehouseId = request.ToWarehouseId,
                    TransactionType = StockTransactionType.TransferOut,
                    Quantity = request.Quantity,
                    UnitCost = operationUnitCost,
                    TransactionDateUtc = request.TransactionDateUtc ?? DateTime.UtcNow,
                    ReferenceType = request.ReferenceType,
                    ReferenceNumber = request.ReferenceNumber,
                    Notes = request.Notes,
                    CreatedAtUtc = DateTime.UtcNow
                };

                await _stockTransactionRepository.AddAsync(transferOut);

                // إضافة للمخزن الوجهة
                var toBalance = await GetOrCreateBalanceAsync(request.ItemId, request.ToWarehouseId);
                toBalance.QuantityOnHand += request.Quantity;
                toBalance.AvailableQuantity = toBalance.QuantityOnHand - toBalance.ReservedQuantity;
                toBalance.LastUpdatedUtc = DateTime.UtcNow;

                if (toBalance.Id == Guid.Empty)
                    await _itemWarehouseBalanceRepository.AddAsync(toBalance);
                else
                    await _itemWarehouseBalanceRepository.UpdateAsync(toBalance);

                var transferIn = new StockTransaction
                {
                    Id = Guid.NewGuid(),
                    ItemId = request.ItemId,
                    WarehouseId = request.ToWarehouseId,
                    RelatedWarehouseId = request.FromWarehouseId,
                    TransactionType = StockTransactionType.TransferIn,
                    Quantity = request.Quantity,
                    UnitCost = operationUnitCost,
                    TransactionDateUtc = request.TransactionDateUtc ?? DateTime.UtcNow,
                    ReferenceType = request.ReferenceType,
                    ReferenceNumber = request.ReferenceNumber,
                    ReferenceId = request.ReferenceId,
                    Notes = request.Notes,
                    CreatedAtUtc = DateTime.UtcNow
                };

                await _stockTransactionRepository.AddAsync(transferIn);

                if (trx != null) await trx.CommitAsync();
            }
            catch
            {
                if (trx != null) await trx.RollbackAsync();
                throw;
            }
            finally
            {
                trx?.Dispose();
            }
        }

        // =========================
        // Internal helpers
        // =========================

        private async Task ExecuteIncreaseAsync(StockOperationRequest request, StockTransactionType type)
        {
            ValidateRequest(request);

            var item = await _itemRepository.GetByIdAsync(request.ItemId);
            if (item == null)
                throw new InvalidOperationException("الصنف غير موجود");

            if (!item.IsActive)
                throw new InvalidOperationException("الصنف غير نشط");

            if (item.IsService)
                throw new InvalidOperationException("لا يمكن عمل حركة مخزنية على صنف خدمي");

            if (!item.IsStockItem)
                throw new InvalidOperationException("الصنف غير مخزني");

            var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId);
            if (warehouse == null || !warehouse.IsActive)
                throw new InvalidOperationException("المخزن غير موجود أو غير نشط");

            var hasOuterTransaction = _db.Database.CurrentTransaction != null;
            IDbContextTransaction? trx = null;

            if (!hasOuterTransaction)
                trx = await _db.Database.BeginTransactionAsync();

            try
            {
                var balance = await GetOrCreateBalanceAsync(request.ItemId, request.WarehouseId);

                balance.QuantityOnHand += request.Quantity;
                balance.AvailableQuantity = balance.QuantityOnHand - balance.ReservedQuantity;
                balance.LastUpdatedUtc = DateTime.UtcNow;

                if (balance.Id == Guid.Empty)
                    await _itemWarehouseBalanceRepository.AddAsync(balance);
                else
                    await _itemWarehouseBalanceRepository.UpdateAsync(balance);

                var transaction = new StockTransaction
                {
                    Id = Guid.NewGuid(),
                    ItemId = request.ItemId,
                    WarehouseId = request.WarehouseId,
                    TransactionType = type,
                    Quantity = request.Quantity,
                    UnitCost = request.UnitCost,
                    ReferenceId = request.ReferenceId,
                    TransactionDateUtc = request.TransactionDateUtc ?? DateTime.UtcNow,
                    ReferenceType = request.ReferenceType,
                    ReferenceNumber = request.ReferenceNumber,
                    Notes = request.Notes,
                    CreatedAtUtc = DateTime.UtcNow
                };

                await _stockTransactionRepository.AddAsync(transaction);

                if (trx != null) await trx.CommitAsync();
            }
            catch
            {
                if (trx != null) await trx.RollbackAsync();
                throw;
            }
            finally
            {
                trx?.Dispose();
            }
        }

        private async Task ExecuteDecreaseAsync(StockOperationRequest request, StockTransactionType type)
        {
            ValidateRequest(request);

            var item = await _itemRepository.GetByIdAsync(request.ItemId);
            if (item == null)
                throw new InvalidOperationException("الصنف غير موجود");

            if (!item.IsActive)
                throw new InvalidOperationException("الصنف غير نشط");

            if (item.IsService)
                throw new InvalidOperationException("لا يمكن عمل حركة مخزنية على صنف خدمي");

            if (!item.IsStockItem)
                throw new InvalidOperationException("الصنف غير مخزني");

            var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId);
            if (warehouse == null || !warehouse.IsActive)
                throw new InvalidOperationException("المخزن غير موجود أو غير نشط");

            var hasOuterTransaction = _db.Database.CurrentTransaction != null;
            IDbContextTransaction? trx = null;

            if (!hasOuterTransaction)
                trx = await _db.Database.BeginTransactionAsync();

            try
            {
                var balance = await GetOrCreateBalanceAsync(request.ItemId, request.WarehouseId);
                EnsureEnoughStock(balance, request.Quantity);

                balance.QuantityOnHand -= request.Quantity;
                balance.AvailableQuantity = balance.QuantityOnHand - balance.ReservedQuantity;
                balance.LastUpdatedUtc = DateTime.UtcNow;

                if (balance.Id == Guid.Empty)
                    throw new InvalidOperationException("لا يوجد رصيد لهذا الصنف داخل المخزن");

                await _itemWarehouseBalanceRepository.UpdateAsync(balance);
                var operationUnitCost = request.UnitCost;

                if (operationUnitCost <= 0)
                {
                    operationUnitCost = await CalculateMovingAverageUnitCostAsync(
                        request.ItemId,
                        request.WarehouseId,
                        request.TransactionDateUtc ?? DateTime.UtcNow);
                }

                var transaction = new StockTransaction
                {
                    Id = Guid.NewGuid(),
                    ItemId = request.ItemId,
                    WarehouseId = request.WarehouseId,
                    TransactionType = type,
                    Quantity = request.Quantity,
                    UnitCost = operationUnitCost,
                    TransactionDateUtc = request.TransactionDateUtc ?? DateTime.UtcNow,
                    ReferenceType = request.ReferenceType,
                    ReferenceNumber = request.ReferenceNumber,

                    // لو طبقت Patch 35C وفيه ReferenceId:
                    // ReferenceId = request.ReferenceId,

                    Notes = request.Notes,
                    CreatedAtUtc = DateTime.UtcNow
                };

                await _stockTransactionRepository.AddAsync(transaction);

                if (trx != null)
                    await trx.CommitAsync();
            }
            catch
            {
                if (trx != null)
                    await trx.RollbackAsync();
                throw;
            }
        }

        private async Task<ItemWarehouseBalance> GetOrCreateBalanceAsync(Guid itemId, Guid warehouseId)
        {
            var balance = await _itemWarehouseBalanceRepository.GetByItemAndWarehouseAsync(itemId, warehouseId);
            if (balance != null)
                return balance;

            return new ItemWarehouseBalance
            {
                Id = Guid.Empty, // علامة إنه جديد
                ItemId = itemId,
                WarehouseId = warehouseId,
                QuantityOnHand = 0,
                ReservedQuantity = 0,
                AvailableQuantity = 0,
                LastUpdatedUtc = DateTime.UtcNow
            };
        }

        private static void EnsureEnoughStock(ItemWarehouseBalance balance, decimal quantity)
        {
            if (balance.Id == Guid.Empty)
                throw new InvalidOperationException("لا يوجد رصيد لهذا الصنف داخل المخزن");

            if (balance.AvailableQuantity < quantity)
                throw new InvalidOperationException("الكمية المطلوبة أكبر من الرصيد المتاح");
        }

        private static void ValidateRequest(StockOperationRequest request)
        {
            if (request.ItemId == Guid.Empty)
                throw new InvalidOperationException("الصنف مطلوب");

            if (request.WarehouseId == Guid.Empty)
                throw new InvalidOperationException("المخزن مطلوب");

            if (request.Quantity <= 0)
                throw new InvalidOperationException("الكمية يجب أن تكون أكبر من صفر");

            if (request.UnitCost < 0)
                throw new InvalidOperationException("التكلفة لا يمكن أن تكون سالبة");
        }
        private static bool IsIncreaseType(StockTransactionType type)
        {
            return type == StockTransactionType.OpeningBalance
                || type == StockTransactionType.Purchase
                || type == StockTransactionType.SaleReturn
                || type == StockTransactionType.TransferIn
                || type == StockTransactionType.AdjustmentIncrease;
        }

        private static bool IsDecreaseType(StockTransactionType type)
        {
            return type == StockTransactionType.PurchaseReturn
                || type == StockTransactionType.Sale
                || type == StockTransactionType.TransferOut
                || type == StockTransactionType.AdjustmentDecrease;
        }

        private async Task<decimal> CalculateMovingAverageUnitCostAsync(
            Guid itemId,
            Guid warehouseId,
            DateTime? asOfDateUtc = null)
        {
            var asOf = asOfDateUtc ?? DateTime.UtcNow;

            var transactions = await _db.Set<StockTransaction>()
                .AsNoTracking()
                .Where(x => x.ItemId == itemId
                    && x.WarehouseId == warehouseId
                    && x.TransactionDateUtc <= asOf)
                .OrderBy(x => x.TransactionDateUtc)
                .ThenBy(x => x.CreatedAtUtc)
                .ToListAsync();

            decimal qty = 0;
            decimal value = 0;
            decimal lastKnownCost = 0;

            foreach (var trx in transactions)
            {
                if (IsIncreaseType(trx.TransactionType))
                {
                    var unitCost = trx.UnitCost > 0 ? trx.UnitCost : lastKnownCost;

                    qty += trx.Quantity;
                    value += trx.Quantity * unitCost;

                    if (unitCost > 0)
                        lastKnownCost = unitCost;
                }
                else if (IsDecreaseType(trx.TransactionType))
                {
                    var avgCost = qty > 0 ? value / qty : lastKnownCost;
                    var unitCost = trx.UnitCost > 0 ? trx.UnitCost : avgCost;

                    qty -= trx.Quantity;
                    value -= trx.Quantity * unitCost;

                    if (unitCost > 0)
                        lastKnownCost = unitCost;

                    if (qty <= 0)
                    {
                        qty = 0;
                        value = 0;
                    }
                }
            }

            if (qty > 0 && value > 0)
                return Math.Round(value / qty, 4);

            return Math.Round(lastKnownCost, 4);
        }

        private static void ValidateTransferRequest(StockTransferRequest request)
        {
            if (request.ItemId == Guid.Empty)
                throw new InvalidOperationException("الصنف مطلوب");

            if (request.FromWarehouseId == Guid.Empty)
                throw new InvalidOperationException("مخزن الصرف مطلوب");

            if (request.ToWarehouseId == Guid.Empty)
                throw new InvalidOperationException("مخزن الإضافة مطلوب");

            if (request.FromWarehouseId == request.ToWarehouseId)
                throw new InvalidOperationException("لا يمكن التحويل لنفس المخزن");

            if (request.Quantity <= 0)
                throw new InvalidOperationException("الكمية يجب أن تكون أكبر من صفر");

            if (request.UnitCost < 0)
                throw new InvalidOperationException("التكلفة لا يمكن أن تكون سالبة");
        }
    }
}
