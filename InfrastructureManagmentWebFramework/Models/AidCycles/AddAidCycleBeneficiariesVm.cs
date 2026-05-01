namespace InfrastructureManagmentWebFramework.Models.Charity.AidCycles;

public class AddAidCycleBeneficiariesVm
{
    public Guid AidCycleId { get; set; }
    public string? AidCycleTitle { get; set; }
    public List<Guid> SelectedBeneficiaryIds { get; set; } = new();
    public decimal? ApprovedAmount { get; set; }
    public string? Notes { get; set; }
    public IReadOnlyList<AidCycleEligibleBeneficiaryVm> Items { get; set; } = Array.Empty<AidCycleEligibleBeneficiaryVm>();
}
