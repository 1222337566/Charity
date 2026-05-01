namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.Shared;

public class BeneficiaryHeaderVm
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StatusName { get; set; }
}
