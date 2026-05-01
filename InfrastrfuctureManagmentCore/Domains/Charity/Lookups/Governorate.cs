namespace InfrastrfuctureManagmentCore.Domains.Charity.Lookups
{
    public class Governorate
    {
        public Guid Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string? NameEn { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}
