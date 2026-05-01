using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Accounting.Integration;
using InfrastrfuctureManagmentCore.Domains.Purchase;
using InfrastrfuctureManagmentCore.Domains.Sale;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class OperationalJournalHookService : IOperationalJournalHookService
    {
        private readonly AppDbContext _db;
        private readonly IOperationalJournalService _operationalJournalService;

        public OperationalJournalHookService(AppDbContext db, IOperationalJournalService operationalJournalService)
        {
            _db = db;
            _operationalJournalService = operationalJournalService;
        }

        public Task<OperationalJournalHookResult> TryCreateDonationEntryAsync(Guid donationId) =>
            TryCreateAsync(AccountingSourceTypes.Donation, donationId, () => _operationalJournalService.CreateDonationEntryAsync(donationId), "تم إنشاء القيد المحاسبي للتبرع تلقائيًا");
        public Task<OperationalJournalHookResult> TryCreateExpenseEntryAsync(Guid expenseId) =>
            TryCreateAsync(AccountingSourceTypes.Expense, expenseId, () => _operationalJournalService.CreateExpenseEntryAsync(expenseId), "تم إنشاء القيد المحاسبي للمصروف تلقائيًا");
        public Task<OperationalJournalHookResult> TryCreatePayrollEntryAsync(Guid payrollMonthId) =>
            TryCreateAsync(AccountingSourceTypes.Payroll, payrollMonthId, () => _operationalJournalService.CreatePayrollEntryAsync(payrollMonthId), "تم إنشاء قيد المرتبات تلقائيًا");
        public Task<OperationalJournalHookResult> TryCreateStoreIssueEntryAsync(Guid storeIssueId) =>
            TryCreateAsync(AccountingSourceTypes.StoreIssue, storeIssueId, () => _operationalJournalService.CreateStoreIssueEntryAsync(storeIssueId), "تم إنشاء قيد الصرف المخزني تلقائيًا");
        public Task<OperationalJournalHookResult> TryCreateStoreReceiptEntryAsync(Guid storeReceiptId) =>
            TryCreateAsync(AccountingSourceTypes.StoreReceipt, storeReceiptId, () => _operationalJournalService.CreateStoreReceiptEntryAsync(storeReceiptId), "تم إنشاء قيد الإضافة المخزنية تلقائيًا");
        public Task<OperationalJournalHookResult> TryCreateGrantInstallmentEntryAsync(Guid grantInstallmentId) =>
            TryCreateAsync(AccountingSourceTypes.GrantInstallment, grantInstallmentId, () => _operationalJournalService.CreateGrantInstallmentEntryAsync(grantInstallmentId), "تم إنشاء قيد استلام دفعة التمويل تلقائيًا");
        public Task<OperationalJournalHookResult> TryCreateBeneficiaryAidDisbursementEntryAsync(Guid aidDisbursementId) =>
            TryCreateAsync(AccountingSourceTypes.BeneficiaryAidDisbursement, aidDisbursementId, () => _operationalJournalService.CreateBeneficiaryAidDisbursementEntryAsync(aidDisbursementId), "تم إنشاء قيد صرف المساعدة تلقائيًا");
        public Task<OperationalJournalHookResult> TryCreatePurchaseInvoiceEntryAsync(Guid purchaseInvoiceId) =>
    TryCreateAsync(
        AccountingSourceTypes.PurchaseInvoice,
        purchaseInvoiceId,
        () => _operationalJournalService.CreateDynamicEntryAsync(
            AccountingSourceTypes.PurchaseInvoice,
            purchaseInvoiceId),
        "تم إنشاء القيد المحاسبي لفاتورة الشراء تلقائيًا");
        public Task<OperationalJournalHookResult> TryCreateSalesInvoiceCogsEntryAsync(Guid salesInvoiceId) =>
    TryCreateAsync(
        AccountingSourceTypes.SalesInvoiceCOGS,
        salesInvoiceId,
        () => _operationalJournalService.CreateSalesInvoiceCogsEntryAsync(salesInvoiceId),
        "تم إنشاء قيد تكلفة البضاعة المباعة تلقائيًا");

        public Task<OperationalJournalHookResult> TryCreateSalesInvoiceEntryAsync(Guid salesInvoiceId) =>
            TryCreateAsync(
                AccountingSourceTypes.SalesInvoice,
                salesInvoiceId,
                () => _operationalJournalService.CreateDynamicEntryAsync(
                    AccountingSourceTypes.SalesInvoice,
                    salesInvoiceId),
                "تم إنشاء القيد المحاسبي لفاتورة البيع تلقائيًا");
        private async Task<OperationalJournalHookResult> TryCreateAsync(string sourceType, Guid sourceId, Func<Task<JournalEntry>> factory, string successMessage)
        {
            try
            {
                var existing = await _db.Set<JournalEntry>().AsNoTracking().FirstOrDefaultAsync(x => x.SourceType == sourceType && x.SourceId == sourceId && x.Status != JournalEntryStatus.Reversed);
                if (existing != null) return OperationalJournalHookResult.AlreadyExists(existing.Id, $"يوجد قيد محاسبي سابق لهذه الحركة برقم {existing.EntryNumber}");
                var entry = await factory();
                return OperationalJournalHookResult.Created(entry.Id, $"{successMessage} برقم {entry.EntryNumber}");
            }
            catch (Exception ex)
            {
                return OperationalJournalHookResult.Failed($"تم حفظ الحركة، لكن تعذر إنشاء القيد المحاسبي تلقائيًا: {ex.Message}");
            }
        }
      
    }
}
