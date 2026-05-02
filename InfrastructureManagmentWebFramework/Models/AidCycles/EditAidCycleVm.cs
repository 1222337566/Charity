namespace InfrastructureManagmentWebFramework.Models.Charity.AidCycles
{
    public class EditAidCycleVm : CreateAidCycleVm
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = "Draft";
    }
}
