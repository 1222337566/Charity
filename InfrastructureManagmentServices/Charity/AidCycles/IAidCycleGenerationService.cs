namespace InfrastructureManagmentServices.Charity.AidCycles
{
    public class AidCycleGenerationResult
    {
        public int AddedCount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<string> Messages { get; set; } = new();
    }

    public interface IAidCycleGenerationService
    {
        Task<AidCycleGenerationResult> GenerateAsync(Guid aidCycleId, bool clearExistingDraftLines = false);
    }
}
