namespace InfrastructureManagmentWebFramework.Models.AccountingIntegration
{
    public class OperationalPostingSectionVm
    {
        public string Title { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public List<OperationalPostingRowVm> Rows { get; set; } = new();
        public string? SourceType { get; set; }
    }
}
