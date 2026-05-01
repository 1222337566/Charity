namespace InfrastructureManagmentWebFramework.Models.Charity.AidCycles;

public class AidCycleEligibleBeneficiaryVm
{
    public Guid BeneficiaryId { get; set; }
    public string? Code { get; set; }
    public string? FullName { get; set; }
    public string? NationalId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StatusName { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public string? AidType { get; set; }
    public Guid CommitteeDecisionId { get; set; }
}
