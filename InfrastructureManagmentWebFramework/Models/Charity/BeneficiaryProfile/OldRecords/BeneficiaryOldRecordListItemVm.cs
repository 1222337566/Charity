namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.OldRecords
{
    public class BeneficiaryOldRecordListItemVm
    {
        public Guid Id { get; set; }
        public DateTime RecordDate { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Details { get; set; }
        public bool IsActive { get; set; }
    }
}
