namespace InfrastrfuctureManagmentCore.Domains.Suppliers
{
    public class SupplierPayment
    {
        public Guid Id { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        public Guid SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public Guid? PurchaseInvoiceId { get; set; }
        public InfrastrfuctureManagmentCore.Domains.Purchase.PurchaseInvoice? PurchaseInvoice { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Today;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "Cash"; // Cash|BankTransfer|Cheque
        public string? ChequeNumber { get; set; }
        public string? BankName { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "Paid"; // Paid|Pending|Cancelled
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
