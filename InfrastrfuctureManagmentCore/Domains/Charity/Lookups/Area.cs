namespace InfrastrfuctureManagmentCore.Domains.Charity.Lookups
{
    public class Area
    {
        public Guid Id { get; set; }
        public Guid CityId { get; set; }
        public City? City { get; set; }

        public string NameAr { get; set; } = string.Empty;
        public string? NameEn { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
