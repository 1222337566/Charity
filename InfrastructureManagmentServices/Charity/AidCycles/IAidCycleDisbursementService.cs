using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentServices.Charity.AidCycles
{
    public class AidCycleDisbursementRequest
    {
        public Guid AidCycleId { get; set; }
        public DateTime DisbursementDate { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public Guid? FinancialAccountId { get; set; }
        public List<Guid> LineIds { get; set; } = new();
        public string? Notes { get; set; }
        public string? UserId { get; set; }
    }

    public class AidCycleDisbursementResult
    {
        public int DisbursedCount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<string> Messages { get; set; } = new();
    }

    public interface IAidCycleDisbursementService
    {
        Task<AidCycleDisbursementResult> DisburseAsync(AidCycleDisbursementRequest request);
    }
}
