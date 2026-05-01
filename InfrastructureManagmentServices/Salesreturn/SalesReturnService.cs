using InfrastrfuctureManagmentCore.Domains.SalesReturn;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Sale;
using InfrastrfuctureManagmentCore.Persistence.Repositories.salesreturn;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Stock;
using InfrastructureManagmentWebFramework.Models.Salesreturn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Salesreturn
{
    public class SalesReturnService : ISalesReturnService
    {
        private readonly AppDbContext _db;
        private readonly ISalesInvoiceRepository _salesInvoiceRepository;
        private readonly ISalesReturnRepository _salesReturnRepository;
        private readonly IStockService _stockService;

        public SalesReturnService(
            AppDbContext db,
            ISalesInvoiceRepository salesInvoiceRepository,
            ISalesReturnRepository salesReturnRepository,
            IStockService stockService)
        {
            _db = db;
            _salesInvoiceRepository = salesInvoiceRepository;
            _salesReturnRepository = salesReturnRepository;
            _stockService = stockService;
        }

        public async Task<Guid> CreateAsync(CreateSalesReturnVm vm)
        {
            if (string.IsNullOrWhiteSpace(vm.ReturnNumber))
                throw new InvalidOperationException("رقم المرتجع مطلوب");

            if (!vm.Lines.Any(x => x.ReturnQuantity > 0))
                throw new InvalidOperationException("أدخل صنفًا واحدًا على الأقل بكمية مرتجعة");

            if (await _salesReturnRepository.ReturnNumberExistsAsync(vm.ReturnNumber.Trim()))
                throw new InvalidOperationException("رقم المرتجع موجود بالفعل");

            var originalInvoice = await _salesInvoiceRepository.GetByIdAsync(vm.OriginalSalesInvoiceId);
            if (originalInvoice == null)
                throw new InvalidOperationException("فاتورة البيع الأصلية غير موجودة");

            await using var trx = await _db.Database.BeginTransactionAsync();

            try
            {
                var invoice = new SalesReturnInvoice
                {
                    Id = Guid.NewGuid(),
                    ReturnNumber = vm.ReturnNumber.Trim(),
                    ReturnDateUtc = vm.ReturnDateUtc,
                    OriginalSalesInvoiceId = originalInvoice.Id,
                    CustomerName = originalInvoice.CustomerName,
                    WarehouseId = originalInvoice.WarehouseId,
                    Notes = vm.Notes?.Trim(),
                    Status = SalesReturnStatus.Posted,
                    CreatedAtUtc = DateTime.UtcNow
                };

                decimal subTotal = 0;
                decimal discount = 0;
                decimal tax = 0;

                foreach (var lineVm in vm.Lines.Where(x => x.ReturnQuantity > 0))
                {
                    var originalLine = originalInvoice.Lines.FirstOrDefault(x => x.Id == lineVm.OriginalSalesInvoiceLineId);
                    if (originalLine == null)
                        throw new InvalidOperationException("أحد سطور الفاتورة الأصلية غير موجود");

                    var returnedBefore = await _salesReturnRepository.GetReturnedQtyForOriginalLineAsync(originalLine.Id);
                    var remaining = originalLine.Quantity - returnedBefore;

                    if (lineVm.ReturnQuantity > remaining)
                        throw new InvalidOperationException($"الكمية المرتجعة للصنف {lineVm.ItemNameAr} أكبر من المتبقي المسموح");

                    var proportionalDiscount = originalLine.Quantity == 0 ? 0 : (originalLine.DiscountAmount / originalLine.Quantity) * lineVm.ReturnQuantity;
                    var proportionalTax = originalLine.Quantity == 0 ? 0 : (originalLine.TaxAmount / originalLine.Quantity) * lineVm.ReturnQuantity;
                    var lineTotal = (lineVm.ReturnQuantity * originalLine.UnitPrice) - proportionalDiscount + proportionalTax;

                    invoice.Lines.Add(new SalesReturnLine
                    {
                        Id = Guid.NewGuid(),
                        OriginalSalesInvoiceLineId = originalLine.Id,
                        ItemId = originalLine.ItemId,
                        Quantity = lineVm.ReturnQuantity,
                        UnitPrice = originalLine.UnitPrice,
                        DiscountAmount = proportionalDiscount,
                        TaxAmount = proportionalTax,
                        LineTotal = lineTotal
                    });

                    subTotal += lineVm.ReturnQuantity * originalLine.UnitPrice;
                    discount += proportionalDiscount;
                    tax += proportionalTax;
                }

                invoice.SubTotal = subTotal;
                invoice.DiscountAmount = discount;
                invoice.TaxAmount = tax;
                invoice.NetAmount = subTotal - discount + tax;

                await _salesReturnRepository.AddAsync(invoice);

                foreach (var line in invoice.Lines)
                {
                    await _stockService.AddSaleReturnAsync(new StockOperationRequest
                    {
                        ItemId = line.ItemId,
                        WarehouseId = invoice.WarehouseId,
                        Quantity = line.Quantity,
                        UnitCost = 0,
                        TransactionDateUtc = invoice.ReturnDateUtc,
                        ReferenceType = "SalesReturn",
                        ReferenceNumber = invoice.ReturnNumber,
                        Notes = $"مرتجع للفاتورة الأصلية: {originalInvoice.InvoiceNumber}"
                    });
                }

                await trx.CommitAsync();
                return invoice.Id;
            }
            catch
            {
                await trx.RollbackAsync();
                throw;
            }
        }
    }
}
