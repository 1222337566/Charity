using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Skote.Models.Charity.MinutesAndDecisions
{
    public class BoardDecisionAttachmentVm
    {
        [Required]
        public Guid BoardDecisionId { get; set; }

        [Display(Name = "نوع الملف")]
        [StringLength(100)]
        public string? AttachmentType { get; set; }

        [Display(Name = "ملاحظات")]
        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "اختر ملفًا للرفع.")]
        [Display(Name = "الملف")]
        public IFormFile? File { get; set; }
    }
}
