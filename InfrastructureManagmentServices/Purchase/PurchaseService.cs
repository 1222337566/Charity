using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Purchase;
using InfrastrfuctureManagmentCore.Domains.Suppliers;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Purchase;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Suppliers;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Stock;
using InfrastructureManagmentWebFramework.Models.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Purchase
{
    public class PurchaseService : IPurchaseService
    {
        private readonly AppDbContext _db;
        private readonly IItemRepository _itemRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository;
        private readonly IStockService _stockService;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICharityProjectRepository _charityProjectRepository;
        private readonly IGrantAgreementRepository _grantAgreementRepository;
        private readonly IOperationalJournalHookService _journalHook;

        public PurchaseService(
            AppDbContext db,
            IItemRepository itemRepository,
            IWarehouseRepository warehouseRepository,
            IPurchaseInvoiceRepository purchaseInvoiceRepository,
            IStockService stockService,
            ISupplierRepository supplierRepository,
            ICharityProjectRepository charityProjectRepository,
            IGrantAgreementRepository grantAgreementRepository,
            IOperationalJournalHookService operationalJournalHookService)
        {
            _db = db;
            _itemRepository = itemRepository;
            _warehouseRepository = warehouseRepository;
            _purchaseInvoiceRepository = purchaseInvoiceRepository;
            _stockService = stockService;
            _supplierRepository = supplierRepository;
            _charityProjectRepository = charityProjectRepository;
            _grantAgreementRepository = grantAgreementRepository;
            _journalHook = operationalJournalHookService;
        }
        private static string BuildPurchaseReceiptNumber(string invoiceNumber)
        {
            var safe = string.IsNullOrWhiteSpace(invoiceNumber)
                ? DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                : invoiceNumber.Trim();

            var number = $"GRN-PI-{safe}";
            return number.Length <= 50 ? number : number[..50];
        }

        public async Task CreateAsync(CreatePurchaseInvoiceVm vm)
        {
            if (string.IsNullOrWhiteSpace(vm.InvoiceNumber))
                throw new InvalidOperationException("رقم الفاتورة مطلوب");

            if (string.IsNullOrWhiteSpace(vm.SupplierName))
                throw new InvalidOperationException("اسم المورد مطلوب");

            if (!vm.Lines.Any())
                throw new InvalidOperationException("الفاتورة يجب أن تحتوي على صنف واحد على الأقل");

            if (await _purchaseInvoiceRepository.InvoiceNumberExistsAsync(vm.InvoiceNumber.Trim()))
                throw new InvalidOperationException("رقم الفاتورة موجود بالفعل");

            var warehouse = await _warehouseRepository.GetByIdAsync(vm.WarehouseId);
            if (warehouse == null || !warehouse.IsActive)
                throw new InvalidOperationException("المخزن غير موجود أو غير نشط");

            await using var trx = await _db.Database.BeginTransactionAsync();

            try
            {
                Supplier? supplier = null;
                string? projectName = null;
                string? grantAgreementNumber = null;

                if (vm.ProjectId.HasValue && vm.ProjectId.Value != Guid.Empty)
                {
                    var project = await _charityProjectRepository.GetByIdAsync(vm.ProjectId.Value);
                    if (project == null || !project.IsActive)
                        throw new InvalidOperationException("المشروع المرتبط غير موجود أو غير نشط");

                    projectName = project.Name;
                }

                if (vm.GrantAgreementId.HasValue && vm.GrantAgreementId.Value != Guid.Empty)
                {
                    var agreement = await _grantAgreementRepository.GetByIdAsync(vm.GrantAgreementId.Value);
                    if (agreement == null)
                        throw new InvalidOperationException("اتفاقية التمويل المختارة غير موجودة");

                    grantAgreementNumber = agreement.AgreementNumber;
                }

                if (vm.SupplierId.HasValue && vm.SupplierId.Value != Guid.Empty)
                {
                    supplier = await _supplierRepository.GetByIdAsync(vm.SupplierId.Value);
                    if (supplier == null || !supplier.IsActive)
                        throw new InvalidOperationException("المورد غير موجود أو غير نشط");
                }
                var invoice = new PurchaseInvoice
                {
                    Id = Guid.NewGuid(),
                    InvoiceNumber = vm.InvoiceNumber.Trim(),
                    InvoiceDateUtc = vm.InvoiceDateUtc,
                    SupplierId = supplier?.Id,
                    SupplierName = supplier != null ? supplier.NameAr : vm.SupplierName.Trim(),
                    SupplierInvoiceNumber = vm.SupplierInvoiceNumber?.Trim(),
                    WarehouseId = vm.WarehouseId,
                    Notes = InfrastructureManagmentWebFramework.Helpers.CharityOperationNoteTags.Merge(
                        vm.Notes,
                        vm.ProcurementCategory,
                        projectName,
                        grantAgreementNumber,
                        false),
                    Status = PurchaseInvoiceStatus.Posted,
                    CreatedAtUtc = DateTime.UtcNow
                };

                decimal subTotal = 0;
                decimal discount = 0;
                decimal tax = 0;

                foreach (var line in vm.Lines.Where(x => x.ItemId != Guid.Empty && x.Quantity > 0))
                {
                    var item = await _itemRepository.GetByIdAsync(line.ItemId);
                    if (item == null)
                        throw new InvalidOperationException("أحد الأصناف غير موجود");

                    var lineTotal = (line.Quantity * line.UnitCost) - line.DiscountAmount + line.TaxAmount;

                    invoice.Lines.Add(new PurchaseInvoiceLine
                    {
                        Id = Guid.NewGuid(),
                        ItemId = line.ItemId,
                        Quantity = line.Quantity,
                        UnitCost = line.UnitCost,
                        DiscountAmount = line.DiscountAmount,
                        TaxAmount = line.TaxAmount,
                        LineTotal = lineTotal
                    });

                    subTotal += line.Quantity * line.UnitCost;
                    discount += line.DiscountAmount;
                    tax += line.TaxAmount;
                }

                invoice.SubTotal = subTotal;
                invoice.DiscountAmount = discount;
                invoice.TaxAmount = tax;
                invoice.NetAmount = subTotal - discount + tax;

                await _purchaseInvoiceRepository.AddAsync(invoice);
                var receipt = new CharityStoreReceipt
                {
                    Id = Guid.NewGuid(),
                    ReceiptNumber = BuildPurchaseReceiptNumber(invoice.InvoiceNumber),
                    WarehouseId = invoice.WarehouseId,
                    ReceiptDate = invoice.InvoiceDateUtc.Date,
                    SourceType = "PurchaseInvoice",
                    SourceId = invoice.Id,
                    SourceName = invoice.InvoiceNumber,
                    Notes = $"إذن إضافة تلقائي من فاتورة شراء رقم {invoice.InvoiceNumber}. PurchaseInvoiceId:{invoice.Id}",
                    ApprovalStatus = "Approved",
                    ApprovedAtUtc = DateTime.UtcNow,
                    ApprovedByUserId = null,
                    ApprovalNotes = "اعتماد تلقائي عند إصدار فاتورة الشراء",
                    CreatedAtUtc = DateTime.UtcNow
                };

                foreach (var line in invoice.Lines)
                {
                    receipt.Lines.Add(new CharityStoreReceiptLine
                    {
                        Id = Guid.NewGuid(),
                        ReceiptId = receipt.Id,
                        ItemId = line.ItemId,
                        Quantity = line.Quantity,
                        UnitCost = line.UnitCost,
                        Notes = $"إضافة تلقائية من فاتورة شراء رقم {invoice.InvoiceNumber}"
                    });
                }

                _db.Set<CharityStoreReceipt>().Add(receipt);
                await _db.SaveChangesAsync();

                foreach (var line in receipt.Lines)
                {
                    await _stockService.AddPurchaseAsync(new StockOperationRequest
                    {
                        ItemId = line.ItemId,
                        WarehouseId = receipt.WarehouseId,
                        Quantity = line.Quantity,
                        UnitCost = line.UnitCost,
                        TransactionDateUtc = receipt.ReceiptDate,
                        ReferenceType = nameof(CharityStoreReceipt),
                        ReferenceNumber = receipt.ReceiptNumber,
                        ReferenceId = receipt.Id,
                        Notes = line.Notes ?? receipt.Notes
                    });
                }
                await _journalHook.TryCreatePurchaseInvoiceEntryAsync(invoice.Id);
                await trx.CommitAsync();
                
            }
            catch
            {
                await trx.RollbackAsync();
                throw;
            }
        }
    }
}
