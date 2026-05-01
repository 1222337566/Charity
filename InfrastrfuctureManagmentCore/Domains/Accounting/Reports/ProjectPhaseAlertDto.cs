namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class ProjectPhaseAlertDto
    {
        public DateTime AsOfDate { get; set; }
        public List<ProjectPhaseAlertRowDto> Rows { get; set; } = new();
    }
}
