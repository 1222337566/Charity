namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting
{
    public interface IOperationalJournalHookService
    {
        Task<OperationalJournalHookResult> TryCreateDonationEntryAsync(Guid donationId);
        Task<OperationalJournalHookResult> TryCreateExpenseEntryAsync(Guid expenseId);
        Task<OperationalJournalHookResult> TryCreatePayrollEntryAsync(Guid payrollMonthId);
        Task<OperationalJournalHookResult> TryCreateStoreIssueEntryAsync(Guid storeIssueId);
        Task<OperationalJournalHookResult> TryCreateStoreReceiptEntryAsync(Guid storeReceiptId);
        Task<OperationalJournalHookResult> TryCreateGrantInstallmentEntryAsync(Guid grantInstallmentId);
        Task<OperationalJournalHookResult> TryCreateBeneficiaryAidDisbursementEntryAsync(Guid aidDisbursementId);
        Task<OperationalJournalHookResult> TryCreatePurchaseInvoiceEntryAsync(Guid purchaseInvoiceId);
        Task<OperationalJournalHookResult> TryCreateSalesInvoiceCogsEntryAsync(Guid salesInvoiceId);

        Task<OperationalJournalHookResult> TryCreateSalesInvoiceEntryAsync(Guid salesInvoiceId);
    }
}
