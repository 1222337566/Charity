namespace InfrastructureManagmentWebFramework.Models.Charity.FunderProfile.GrantConditions
{
    public class GrantConditionListItemVm
    {
        public Guid Id { get; set; }
        public string ConditionTitle { get; set; } = string.Empty;
        public string ConditionDetails { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsFulfilled { get; set; }
        public DateTime? FulfilledDate { get; set; }
        public string? Notes { get; set; }
    }
}
