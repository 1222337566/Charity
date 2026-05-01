namespace InfrastrfuctureManagmentCore.Domains.Accounting.Integration
{
    public class AccountingIntegrationSourceDefinition
    {
        public Guid Id { get; set; }
        public string SourceType { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? NameEn { get; set; }
        public string? ModuleCode { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; } = 100;
        public bool IsSystem { get; set; }
        public bool IsActive { get; set; } = true;

        // Dynamic posting map. When enabled, OperationalJournalService can create
        // a two-sided journal entry for this source without a source-specific method.
        public bool IsDynamicPostingEnabled { get; set; }
        public string? EntityClrTypeName { get; set; }
        public string? IdPropertyName { get; set; } = "Id";
        public string? DatePropertyName { get; set; }
        public string? AmountPropertyName { get; set; }
        public string? NumberPropertyName { get; set; }
        public string? TitlePropertyName { get; set; }
        public string? DescriptionTemplate { get; set; }
        public string? FinancialAccountIdPropertyName { get; set; }
        public string? ProjectIdPropertyName { get; set; }
        public string? CostCenterIdPropertyName { get; set; }
        public string? EventCodePropertyName { get; set; }
        public string? DonationTypePropertyName { get; set; }
        public string? TargetingScopeCodePropertyName { get; set; }
        public string? PurposeNamePropertyName { get; set; }
        public string? AidTypeIdPropertyName { get; set; }
        public string? StoreMovementTypePropertyName { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
