namespace InfrastructureManagmentServices.Charity.Kafala
{
    public interface IKafalaAidCycleBridgeService
    {
        Task<(Guid? aidCycleId, int generatedCount, List<string> messages)> CreateDueSponsorshipCycleAsync(DateTime plannedDisbursementDateUtc, Guid? sponsorId = null);
    }
}
