using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Domains.Prescriptions;
using InfrastrfuctureManagmentCore.Domains.Sale;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payments;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Prescriptions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Sale;
using InfrastructureManagmentCore.Domains.Profiling;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Customers;
using InfrastructureManagmentServices.Stock;
using InfrastructureManagmentWebFramework.Models.Sale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Sales
{
    public class SalesService : ISalesService
    {
        private readonly AppDbContext _db;
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly ISalesInvoiceRepository _salesInvoiceRepository;
        private readonly IStockService _stockService;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerAccountService _customerAccountService;
        private readonly ICharityProjectRepository _charityProjectRepository;
        private readonly IGrantAgreementRepository _grantAgreementRepository;
        private readonly IOperationalJournalHookService _journalHook;
        public SalesService(
            AppDbContext db,
            IItemRepository itemRepository,
            IWarehouseRepository warehouseRepository,
            ISalesInvoiceRepository salesInvoiceRepository,
            IStockService stockService,
            IPaymentMethodRepository paymentMethodRepository,
            ICustomerRepository customerRepository,
             ICustomerAccountService customerAccountService,
             IPrescriptionRepository prescriptionRepository,
             ICharityProjectRepository charityProjectRepository,
             IGrantAgreementRepository grantAgreementRepository,
             IOperationalJournalHookService operationalJournalHookService)
        {
            _db = db;
            _itemRepository = itemRepository;
            _warehouseRepository = warehouseRepository;
            _salesInvoiceRepository = salesInvoiceRepository;
            _stockService = stockService;
            _prescriptionRepository = prescriptionRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _customerRepository = customerRepository;
            _customerAccountService = customerAccountService;
            _charityProjectRepository = charityProjectRepository;
            _grantAgreementRepository = grantAgreementRepository;
            _journalHook = operationalJournalHookService;
        }
        private static string BuildSalesIssueNumber(string invoiceNumber)
        {
            var safe = string.IsNullOrWhiteSpace(invoiceNumber)
                ? DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                : invoiceNumber.Trim();

            var number = $"ISS-SI-{safe}";
            return number.Length <= 50 ? number : number[..50];
        }

        public async Task<Guid> CreateAsync(CreateSalesInvoiceVm vm)
        {
            if (string.IsNullOrWhiteSpace(vm.InvoiceNumber))
                throw new InvalidOperationException("رقم الفاتورة مطلوب");

            if (string.IsNullOrWhiteSpace(vm.CustomerName))
                throw new InvalidOperationException("اسم العميل مطلوب");
            
            if (!vm.Lines.Any())
                throw new InvalidOperationException("الفاتورة يجب أن تحتوي على صنف واحد على الأقل");

            if (await _salesInvoiceRepository.InvoiceNumberExistsAsync(vm.InvoiceNumber.Trim()))
                throw new InvalidOperationException("رقم الفاتورة موجود بالفعل");

            var warehouse = await _warehouseRepository.GetByIdAsync(vm.WarehouseId);
            
            if (warehouse == null || !warehouse.IsActive)
                throw new InvalidOperationException("المخزن غير موجود أو غير نشط");

            await using var trx = await _db.Database.BeginTransactionAsync();

            try
            {
                CustomerClient? customer = null;
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

                if (vm.CustomerId.HasValue && vm.CustomerId.Value != Guid.Empty)
                {
                    customer = await _customerRepository.GetByIdAsync(vm.CustomerId.Value);
                    if (customer == null || !customer.IsActive)
                        throw new InvalidOperationException("العميل غير موجود أو غير نشط");
                }
                Prescription? prescription = null;

                if (vm.PrescriptionId.HasValue && vm.PrescriptionId.Value != Guid.Empty)
                {
                    prescription = await _prescriptionRepository.GetByIdAsync(vm.PrescriptionId.Value);
                    if (prescription == null || !prescription.IsActive)
                        throw new InvalidOperationException("الروشتة غير موجودة أو غير نشطة");

                    if (!vm.CustomerId.HasValue || vm.CustomerId.Value == Guid.Empty)
                        throw new InvalidOperationException("لا يمكن ربط روشتة بفاتورة غير مرتبطة بعميل");

                    if (prescription.CustomerId != vm.CustomerId.Value)
                        throw new InvalidOperationException("الروشتة المختارة لا تخص هذا العميل");
                }
                var invoice = new SalesInvoice
                {
                    Id = Guid.NewGuid(),
                    InvoiceNumber = vm.InvoiceNumber.Trim(),
                    InvoiceDateUtc = vm.InvoiceDateUtc,
                    CustomerId = customer?.Id,
                    PrescriptionId = prescription?.Id,
                    CustomerName = customer != null ? customer.NameAr : vm.CustomerName.Trim(),
                    WarehouseId = vm.WarehouseId,
                    Notes = InfrastructureManagmentWebFramework.Helpers.CharityOperationNoteTags.Merge(
                        vm.Notes,
                        vm.RevenueCategory,
                        projectName,
                        grantAgreementNumber,
                        true),
                    Status = SalesInvoiceStatus.Posted,
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

                    if (!item.IsActive)
                        throw new InvalidOperationException($"الصنف {item.ItemNameAr} غير نشط");

                    var lineTotal = (line.Quantity * line.UnitPrice) - line.DiscountAmount + line.TaxAmount;

                    invoice.Lines.Add(new SalesInvoiceLine
                    {
                        Id = Guid.NewGuid(),
                        ItemId = line.ItemId,
                        Quantity = line.Quantity,
                        UnitPrice = line.UnitPrice,
                        DiscountAmount = line.DiscountAmount,
                        TaxAmount = line.TaxAmount,
                        LineTotal = lineTotal
                    });

                    subTotal += line.Quantity * line.UnitPrice;
                    discount += line.DiscountAmount;
                    tax += line.TaxAmount;
                }

                invoice.SubTotal = subTotal;
                invoice.DiscountAmount = discount;
                invoice.TaxAmount = tax;
                invoice.NetAmount = subTotal - discount + tax;
                var payments = vm.Payments
    .Where(x => x.PaymentMethodId != Guid.Empty && x.Amount > 0)
    .ToList();

                if (!payments.Any())
                    throw new InvalidOperationException("أدخل طريقة دفع واحدة على الأقل");

                var totalPaid = payments.Sum(x => x.Amount);
                if (Math.Round(totalPaid, 2) != Math.Round(invoice.NetAmount, 2))
                    throw new InvalidOperationException("مجموع طرق الدفع يجب أن يساوي صافي الفاتورة");

                foreach (var payment in payments)
                {
                    var method = await _paymentMethodRepository.GetByIdAsync(payment.PaymentMethodId);
                    if (method == null || !method.IsActive)
                        throw new InvalidOperationException("إحدى طرق الدفع غير موجودة أو غير نشطة");

                    invoice.Payments.Add(new SalesInvoicePayment
                    {
                        Id = Guid.NewGuid(),
                        PaymentMethodId = payment.PaymentMethodId,
                        Amount = payment.Amount
                    });
                }
                await _salesInvoiceRepository.AddAsync(invoice);
                await _customerAccountService.PostSaleInvoiceAsync(invoice);
                var issue = new CharityStoreIssue
                {
                    Id = Guid.NewGuid(),
                    IssueNumber = BuildSalesIssueNumber(invoice.InvoiceNumber),
                    WarehouseId = invoice.WarehouseId,
                    IssueDate = invoice.InvoiceDateUtc.Date,
                    IssueType = "SalesInvoice",
                    SourceId = invoice.Id,
                    IssuedToName = invoice.CustomerName,
                    Notes = $"إذن صرف تلقائي من فاتورة بيع رقم {invoice.InvoiceNumber}. SalesInvoiceId:{invoice.Id}",
                    ApprovalStatus = "Approved",
                    ApprovedAtUtc = DateTime.UtcNow,
                    ApprovedByUserId = null,
                    ApprovalNotes = "اعتماد تلقائي عند إصدار فاتورة البيع",
                    CreatedAtUtc = DateTime.UtcNow
                };

                foreach (var line in invoice.Lines)
                {
                    issue.Lines.Add(new CharityStoreIssueLine
                    {
                        Id = Guid.NewGuid(),
                        IssueId = issue.Id,
                        ItemId = line.ItemId,
                        Quantity = line.Quantity,
                        UnitCost = 0, // لاحقًا Patch تكلفة المخزون Average/FIFO
                        Notes = $"صرف تلقائي من فاتورة بيع رقم {invoice.InvoiceNumber}"
                    });
                }

                _db.Set<CharityStoreIssue>().Add(issue);
                await _db.SaveChangesAsync();

                foreach (var line in issue.Lines)
                {
                    await _stockService.AddSaleAsync(new StockOperationRequest
                    {
                        ItemId = line.ItemId,
                        WarehouseId = issue.WarehouseId,
                        Quantity = line.Quantity,
                        UnitCost = line.UnitCost,
                        TransactionDateUtc = issue.IssueDate,
                        ReferenceType = nameof(CharityStoreIssue),
                        ReferenceNumber = issue.IssueNumber,
                        ReferenceId = issue.Id,
                        Notes = line.Notes ?? issue.Notes
                    });
                }
                await trx.CommitAsync();
                await _journalHook.TryCreateSalesInvoiceEntryAsync(invoice.Id);
                await _journalHook.TryCreateSalesInvoiceCogsEntryAsync(invoice.Id);

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
