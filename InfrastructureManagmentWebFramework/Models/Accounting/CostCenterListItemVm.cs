namespace InfrastructureManagmentWebFramework.Models.Accounting
{
    public class CostCenterListItemVm
    {
        public Guid Id { get; set; }
        public string CostCenterCode { get; set; } = string.Empty;
        public string CostCenterNameAr { get; set; } = string.Empty;
        public Guid? ParentCostCenterId { get; set; }
        public int Level { get; set; }
        public bool IsProjectRelated { get; set; }
        public bool IsActive { get; set; }
    }
}
