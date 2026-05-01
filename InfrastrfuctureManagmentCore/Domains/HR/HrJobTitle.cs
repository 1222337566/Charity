namespace InfrastrfuctureManagmentCore.Domains.HR
{
    public class HrJobTitle
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        /// <summary>الدور في النظام المرتبط بهذه الوظيفة — يُسند تلقائياً عند إنشاء حساب</summary>
        public string? SystemRole { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<HrEmployee> Employees { get; set; } = new List<HrEmployee>();
    }
}
