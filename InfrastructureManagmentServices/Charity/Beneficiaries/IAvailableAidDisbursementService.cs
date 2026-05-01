using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastructureManagmentServices.Charity.Beneficiaries;


namespace InfrastructureManagmentServices.Charity.Beneficiaries
{
    public interface IAvailableAidDisbursementService
    {
        Task<BeneficiaryAidDisbursement?> EnsureAvailableFromApprovedAllocationAsync(Guid allocationId, string? currentUserId, CancellationToken ct = default);
        Task<BeneficiaryAidDisbursement?> ExecuteCashAsync(Guid disbursementId, string? currentUserId, CancellationToken ct = default);
    }
}
