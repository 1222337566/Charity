using System;
using System.Collections.Generic;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries
{
    public class BeneficiaryCategory
    {
        public Guid Id { get; set; }

        public string NameAr { get; set; } = null!;

        public string? Code { get; set; }

        public string? Description { get; set; }

        public bool IsWaitingListCategory { get; set; }

        public bool RequiresDisabilityType { get; set; }

        public bool IsProjectRelated { get; set; }

        public bool IsActivityRelated { get; set; }

        public bool IsActive { get; set; } = true;

        public int SortOrder { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string? CreatedByUserId { get; set; }

        public ICollection<BeneficiaryCategoryAssignment> Assignments { get; set; }
            = new List<BeneficiaryCategoryAssignment>();
    }
}
