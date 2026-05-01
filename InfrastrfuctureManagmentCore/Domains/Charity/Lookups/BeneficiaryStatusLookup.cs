namespace InfrastrfuctureManagmentCore.Domains.Charity.Lookups
{
    public class BeneficiaryStatusLookup
    {
        public Guid Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string? NameEn { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
