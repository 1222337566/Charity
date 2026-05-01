using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Donors
{
    public class Donor
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = string.Empty;
        public string DonorType { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? NationalIdOrTaxNo { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? AddressLine { get; set; }

        public Guid? GovernorateId { get; set; }
        public Governorate? Governorate { get; set; }

        public Guid? CityId { get; set; }
        public City? City { get; set; }

        public Guid? AreaId { get; set; }
        public Area? Area { get; set; }

        public string? PreferredCommunicationMethod { get; set; }
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
    }
}
