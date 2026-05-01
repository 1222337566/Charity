using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics
{
    using System.ComponentModel.DataAnnotations;

    public class EditOldRecordVm
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        public string CustomerNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "التاريخ")]
        public DateTime RecordDateUtc { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "العنوان")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "التفاصيل")]
        public string? Details { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; }
    }
}
