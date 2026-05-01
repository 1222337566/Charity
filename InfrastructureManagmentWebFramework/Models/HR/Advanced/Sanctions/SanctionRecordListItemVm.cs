namespace InfrastructureManagmentWebFramework.Models.HR.Advanced.Sanctions
{
    public class SanctionRecordListItemVm
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string SanctionType { get; set; } = string.Empty;
        public DateTime SanctionDate { get; set; }
        public decimal? Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
