namespace InfrastructureManagmentServices.Charity.Funding
{
    public interface ICharityOperationalStatusService
    {
        Task<AidRequestOperationalStatusSnapshot?> GetAidRequestStatusAsync(Guid aidRequestId, CancellationToken ct = default);
        Task<Dictionary<Guid, AidRequestOperationalStatusSnapshot>> GetAidRequestStatusesAsync(IEnumerable<Guid> aidRequestIds, CancellationToken ct = default);

        Task<DonationOperationalStatusSnapshot?> GetDonationStatusAsync(Guid donationId, CancellationToken ct = default);
        Task<Dictionary<Guid, DonationOperationalStatusSnapshot>> GetDonationStatusesAsync(IEnumerable<Guid> donationIds, CancellationToken ct = default);
    }
}
