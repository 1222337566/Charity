namespace InfrastrfuctureManagmentCore.Domains.HR
{
    public class HrDepartment
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<HrEmployee> Employees { get; set; } = new List<HrEmployee>();
    }
}
