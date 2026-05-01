namespace InfrastrfuctureManagmentCore.Domains.Payroll
{
    public class PayrollMonth
    {
        public Guid Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Status { get; set; } = "Draft"; // Draft / Calculated / Approved / Posted
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }

        public ICollection<PayrollEmployee> Employees { get; set; } = new List<PayrollEmployee>();
    }
}
