namespace InfrastrfuctureManagmentCore.Domains.Charity.Lookups
{
    public class City
    {
        public Guid Id { get; set; }
        public Guid GovernorateId { get; set; }
        public Governorate? Governorate { get; set; }

        public string NameAr { get; set; } = string.Empty;
        public string? NameEn { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Area> Areas { get; set; } = new List<Area>();
    }
}
