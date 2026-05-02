namespace InfrastructureManagmentWebFramework.Models.AccountingIntegration
{
    public class EditAccountingIntegrationSourceDefinitionVm : CreateAccountingIntegrationSourceDefinitionVm
    {
        public Guid Id { get; set; }
        public bool IsSystem { get; set; }
    }
}
