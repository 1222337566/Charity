using InfrastrfuctureManagmentCore.Domains.Accounting;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting
{
    public interface IOperationalJournalService
    {
        Task<JournalEntry> CreateDonationEntryAsync(Guid donationId);
        Task<JournalEntry> CreateExpenseEntryAsync(Guid expenseId);
        Task<JournalEntry> CreatePayrollEntryAsync(Guid payrollMonthId);
        Task<JournalEntry> CreateStoreIssueEntryAsync(Guid storeIssueId);
        Task<JournalEntry> CreateStoreReceiptEntryAsync(Guid storeReceiptId);
        Task<JournalEntry> CreateGrantInstallmentEntryAsync(Guid grantInstallmentId);
        Task<JournalEntry> CreateBeneficiaryAidDisbursementEntryAsync(Guid aidDisbursementId);

        // Generic dynamic posting entry point for newly defined sources.
        Task<JournalEntry> CreateSalesInvoiceCogsEntryAsync(Guid salesInvoiceId);

        Task<JournalEntry> CreateDynamicEntryAsync(string sourceType, Guid sourceId);
    }
}
