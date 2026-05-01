namespace InfrastructureManagmentServices.Reporting
{
    public interface ICharityWordExportService
    {
        byte[] BuildWordDocument(string title, string htmlBody);
    }
}
