using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;

namespace InfrastructureManagmentServices.Charity.Funding
{
    public interface IAidDisbursementFundingLineService
    {
        Task<RebuildAidDisbursementFundingLinesResult> RebuildFundingLinesAsync(
            BeneficiaryAidDisbursement disbursement,
            string? currentUserId = null,
            CancellationToken ct = default);

        Task<Dictionary<Guid, decimal>> GetRemainingAmountByDonationAsync(
            Guid aidRequestId,
            Guid? excludeDisbursementId = null,
            CancellationToken ct = default);

        Task<Dictionary<Guid, string>> BuildDonationTraceSummaryByDisbursementIdsAsync(
            IEnumerable<Guid> disbursementIds,
            CancellationToken ct = default);
    }
}
