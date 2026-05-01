namespace InfrastructureManagmentServices.Charity.Funding
{
    public interface IAidRequestFundingService
    {
        Task<AidRequestFundingSnapshot?> GetSnapshotAsync(
            Guid aidRequestId,
            Guid? excludeAllocationId = null,
            Guid? excludeDisbursementId = null,
            CancellationToken ct = default);

        Task<Dictionary<Guid, AidRequestFundingSnapshot>> GetSnapshotsAsync(
            IEnumerable<Guid> aidRequestIds,
            Guid? excludeAllocationId = null,
            Guid? excludeDisbursementId = null,
            CancellationToken ct = default);
    }
}
